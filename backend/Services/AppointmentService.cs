﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using backend.DAL.Repositories;
using backend.Models;
using backend.Models.DTOs;

namespace backend.Services
{
    //TODO: Look at automatically loading the foreign properties instead of using the convoluted include
    public class AppointmentService : IAppointmentService
    {
        private readonly AppointmentRepository _repo;
        private readonly AccountRepository _accountRepository;
        private readonly IMapper _mapper;

        static readonly Expression<Func<Appointment, object>>[] appointmentIncludes = new Expression<Func<Appointment, object>>[]
        {
            a => a.AccountsRegistered,
            o => o.Organiser
        };

        static readonly string[] accountStringIncludes = new string[]
        {
            "Appointments.Appointment",
            "OrganizedAppointments"
        };
        
        static readonly Expression<Func<Account, object>>[] accountIncludes = new Expression<Func<Account, object>>[]
        {
            a => a.Appointments,
            o => o.OrganizedAppointments
        };

        public AppointmentService(AppointmentRepository repo, AccountRepository accountRepository, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
            _accountRepository = accountRepository;
        }

        public AppointmentDto GetById(long id)
        {
            Appointment appointment = _repo.GetEntities(x => x.Id == id, appointmentIncludes).FirstOrDefault();
            return _mapper.Map<AppointmentDto>(appointment);
        }

        public IEnumerable<AppointmentDto> GetAll()
        {
            IEnumerable<Appointment> appointments = _repo.GetEntities(includes: appointmentIncludes);

            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public IEnumerable<AppointmentDto> GetWithinTimeSpan(AppointmentsWithinTimespanDto dto)
        {
            var appointments = _repo.GetEntities<Appointment>(
                a => a.BeginTime >= dto.BeginTime && a.EndTime <= dto.EndTime
            ).ToList();

            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public bool RegisterForAppointment(RegisterForAppointmentDto dto)
        {
            Appointment appointment = _repo.GetEntities(x => x.Id == dto.AppointmentId, appointmentIncludes).FirstOrDefault();

            var aa = _mapper.Map<AppointmentAccount>(dto);

            if (IsRegisteredForAppointment(dto) || appointment == null ||
                appointment.AccountsRegistered.Count + 1 > appointment.MaxPeople)
            {
                return false;
            }
            else
            {
                Account account = _accountRepository.GetEntitiesWithStringInclude<Account>(a => a.Id == dto.AccountId, accountStringIncludes).FirstOrDefault();
                if (!IsOverlapping(account, appointment))
                {
                    appointment.AccountsRegistered.Add(aa);

                    // Add appointment to account.
                    if (account.Appointments == null)
                    {
                        account.Appointments = new List<AppointmentAccount>();
                    }
                    account.Appointments.Add(aa);

                    // Save changes.
                    _repo.UpdateEntity(appointment);
                    _accountRepository.UpdateEntity(account);
                    _repo.Save();
                    _accountRepository.Save();

                    return true;
                }
                return false;
            }
        }

        public void Create(CreateAppointmentDto dto)
        {
            Account organiser = _accountRepository.GetEntities(x => x.Id == dto.Organiser, includes:accountIncludes).FirstOrDefault();
            Appointment appointment = _mapper.Map<Appointment>(dto);
            appointment.Organiser = organiser;
            _repo.InsertEntity(appointment);
            _repo.Save();
        }

        public void Update(UpdateAppointmentDto dto)
        {
            var appointment = _mapper.Map<Appointment>(dto);
            _repo.UpdateEntity(appointment);
            _repo.Save();
        }

        public void Delete(AppointmentDto dto)
        {
            Appointment appointment = _repo.GetEntities(x => x.Id == dto.Id, appointmentIncludes).FirstOrDefault();
            _repo.DeleteEntity(appointment);
            _repo.Save();
        }

        public void Unsubscribe(RegisterForAppointmentDto dto)
        {
            Appointment appointment = _repo.GetEntities(x => x.Id == dto.AppointmentId, appointmentIncludes).FirstOrDefault();

            appointment.AccountsRegistered.Remove(appointment.AccountsRegistered.FirstOrDefault(
                x => x.AccountId == dto.AccountId && x.AppointmentId == dto.AppointmentId
            ));

            _repo.UpdateEntity(appointment);
            _repo.Save();
        }

        public bool IsRegisteredForAppointment(RegisterForAppointmentDto registerForAppointmentDto)
        {
            Appointment appointment = _repo.GetEntities(a => a.Id == registerForAppointmentDto.AppointmentId, appointmentIncludes).FirstOrDefault();

            if (appointment == null) return false;

            return appointment.AccountsRegistered.Any(a => a.AccountId == registerForAppointmentDto.AccountId);
        }

        public bool IsOverlapping(Account account, Appointment appointment)
        {
            List<AppointmentAccount> appointmentAccounts = account.Appointments.ToList();
            List<AppointmentDto> appointments = new List<AppointmentDto>();

            foreach (AppointmentAccount app in appointmentAccounts)
            {
                appointments.Add(GetById(app.AppointmentId));
            }

            foreach (AppointmentDto app in appointments)
            {
                int result1 = DateTime.Compare(app.BeginTime, appointment.EndTime);
                int result2 = DateTime.Compare(app.EndTime, appointment.BeginTime);

                if (result1 <= 0 && result2 >= 0) // Check result1 to be earlier than new appointment and result2 to be later than old appointment.
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<AppointmentDto> GetUserRegisteredFor(long userId)
        {
            Account account = _accountRepository.GetEntitiesWithStringInclude<Account>(a => a.Id == userId, includes: accountStringIncludes).FirstOrDefault();


            var app = account.Appointments.Where(a => DateTime.Compare(a.Appointment.BeginTime, DateTime.Now) > 0);

            var appointments = app.Select(b => b.Appointment);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public IEnumerable<AppointmentDto> GetUserOrganized(long userId)
        {
            Account account = _accountRepository.GetEntitiesWithStringInclude<Account>(a => a.Id == userId, includes: accountStringIncludes).FirstOrDefault();

            if (account != null)
            {
                return _mapper.Map<IEnumerable<AppointmentDto>>(account.OrganizedAppointments);
            }

            return null;
        }
    }
}
