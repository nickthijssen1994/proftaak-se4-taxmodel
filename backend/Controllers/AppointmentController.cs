using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Helpers;
using backend.Models.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Authorize]
	[ApiController]
    [Route("taxbreak/api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAccountService _aService;
        private readonly IMailHelper _mailHelper;
        private readonly IAppointmentService _service;

        public AppointmentController(IAppointmentService service, IAccountService aservice, IMailHelper mailHelper)
        {
            _service = service;
            _aService = aservice;
            _mailHelper = mailHelper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AppointmentDto>> GetAppointments()
        {
            return _service.GetAll().ToList();
        }


        [HttpGet("{id}")]
        public ActionResult<AppointmentDto> GetAppointmentById(long id)
        {
            var appointment = _service.GetById(id);

            if (appointment == null) return NotFound();

            return appointment;
        }

        [HttpGet("upcoming/{userId}")]
        public ActionResult<IEnumerable<AppointmentDto>> GetUsersUpcomingAppointments(long userId)
        {
            var appointments = _service.GetUserRegisteredFor(userId).ToList();

            if (appointments == null)
            {
                return NotFound();
            }

            return appointments;
        }

        [HttpGet("organized/{userId}")]
        public ActionResult<IEnumerable<AppointmentDto>> GetAppointmentsUserOrganized(long userId)
        {
            var appointments = _service.GetUserOrganized(userId).ToList();

            if (appointments == null)
            {
                return NotFound();
            }

            return appointments;
        }



        [HttpGet("getInTimeSpan")]
        public ActionResult<IEnumerable<AppointmentDto>> GetAppointmentsInTimeSpan(AppointmentsWithinTimespanDto dto)
        {
            if (ModelState.IsValid || dto == null || dto.EndTime <= dto.BeginTime) return BadRequest();

            var appointments = _service.GetWithinTimeSpan(dto).ToList();

            if (appointments == null) return BadRequest();

            return appointments;
        }


        [Authorize(Policy = "IsAppointmentOwner")]
        [HttpPut("{id}")]
        public ActionResult<UpdateAppointmentDto> PutAppointment(long id, UpdateAppointmentDto appointment)
        {
            if (!ModelState.IsValid || id != appointment.Id)
            {
                return BadRequest();
            }

            _service.Update(appointment);

            return appointment;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterForAppointmentDto>> RegisterForAppointment(RegisterForAppointmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest();

            var appointment = _service.GetById(dto.AppointmentId);
            var account = _aService.GetById(dto.AccountId);
            bool success = _service.RegisterForAppointment(dto);

            if (success)
            {
                await _mailHelper.SetUpRegisterReminderMail(account.Email, appointment.BeginTime, appointment.Location)
                    .ConfigureAwait(true);
            }

            return dto;
        }

        [HttpPost]
        public async Task<ActionResult<CreateAppointmentDto>> PostAppointment(CreateAppointmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest();

            _service.Create(dto);

            var account = _aService.GetById(dto.Organiser);
            await _mailHelper.SetUpReservationReminderMail(account.Email, dto.BeginTime, dto.Location)
                .ConfigureAwait(true);
            
 
            return dto;
            
        }

        [Authorize(Policy = "IsAppointmentOwner")]
        [HttpDelete("{id}")]
        public ActionResult<AppointmentDto> DeleteAppointment(long id)
        {
            var appointment = _service.GetById(id);
            if (appointment == null) return NotFound();

            _service.Delete(appointment);

            return appointment;
        }

        [HttpPost("unsubscribe")]
        public ActionResult<RegisterForAppointmentDto> Unsubscribe(RegisterForAppointmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest();

            _service.Unsubscribe(dto);

            return dto;
        }

        [HttpGet("isRegisteredForAppointment/{accountId}/{appointmentId}")]
        public ActionResult<bool> IsRegisteredForAppointment(long accountId, long appointmentId)
        {
            if (!ModelState.IsValid) return BadRequest();

            var dto = new RegisterForAppointmentDto
            {
                AccountId = accountId,
                AppointmentId = appointmentId
            };

            return _service.IsRegisteredForAppointment(dto);
        }
    }
}