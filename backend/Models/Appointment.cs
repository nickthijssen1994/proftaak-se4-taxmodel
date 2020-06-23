﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
	[Table("appointment")]
	public class Appointment
	{
		public long Id { get; set; }
		public string Title { get; set; }
		public DateTime BeginTime { get; set; }
		public DateTime EndTime { get; set; }
		public string Location { get; set; }
		public int MaxPeople { get; set; }
		public int MinPeople { get; set; }
        [Column("OrganiserId")]
		public Account Organiser { get; set; }
		public ICollection<Order> Orders { get; set; }
		public ICollection<AppointmentAccount> AccountsRegistered { get; set; }
		public string Description { get; set; }
		public string Type { get; set; }
	}
}

