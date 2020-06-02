import {Component, OnInit} from '@angular/core';
import {AppointmentTestService} from '../../services/appointment-test.service';
import {ActivatedRoute} from '@angular/router';
import {Location} from '@angular/common';

@Component({
  selector: 'app-create-appointment',
  templateUrl: './create-appointment.component.html',
  styleUrls: ['./create-appointment.component.css']
})
export class CreateAppointmentComponent implements OnInit {

  public appointment: { organiser: null; size: null; description: string; location: string; id: null; beginTime: Date; endTime: null; title: string; type: null } = {
    id: null,
    title: '',
    description: '',
    location: '',
    type: null,
    size: null,
    beginTime: new Date(),
    endTime: null,
    organiser: null
  };
  disabled = false;

  constructor(private route: ActivatedRoute, private appointmentTestService: AppointmentTestService,
              private location: Location) {
  }

  ngOnInit(): void {
  }

  goBack(): void {
    this.location.back();
  }

  onChangeDate($event): void {
    this.date = $event.value;
  }

  addTimeToDate() {
    this.appointment.beginTime = this.createDate(new Date(this.date), this.appointment.beginDate).toISOString();
    this.appointment.endTime = this.createDate(new Date(this.date), this.appointment.endDate).toISOString();
    console.log(this.appointment.beginDate);
  }

  createDate(date: Date, time: Date): Date {
    let result = new Date(this.date);
    const dateArray = time.toString().split(':');

    result.setHours(Number(dateArray[0]) + 2);
    result.setMinutes(Number(dateArray[1]));

    result = new Date(result.toISOString());
    console.log(result);
    return result;
  }

  validate(): boolean {

    if (this.appointment.type.toString() === 'true') {
      this.appointment.type = 'Private';
    } else {
      this.appointment.type = 'Public';
    }
    if (this.appointment.title === '') {
      this.notificationService.open('Title not filled', null, {
        duration: 5000,
      });
      return false;
    } else if (this.appointment.title.length <= 4) {
      this.notificationService.open('Title can`t be smaller than 4 characters', null, {
        duration: 5000,
      });
      return false;
    } else if (this.appointment.title.length > 50) {
      this.notificationService.open('Title can`t be greater than 50 characters', null, {
        duration: 5000,
      });
      return false;
    } else if (this.appointment.location.length <= 4) {
      console.log(this.appointment.location.length);
      this.notificationService.open('Location has to be greater than 4 characters', null, {
        duration: 5000,
      });
      return false;
    } else if (this.appointment.location.length > 250) {
      this.notificationService.open('Location has to be smaller than 250 characters', null, {
        duration: 5000,
      });
      return false;
    } else if (this.appointment.minPeople < 2) {
      this.notificationService.open('Appointment need more than 1 person', null, {
        duration: 5000,
      });
      return false;
    } else if (this.appointment.maxPeople) {
      if (this.appointment.maxPeople <= this.appointment.minPeople) {
        this.notificationService.open('Minimum people needs to be smaller than the maximum people', null, {
          duration: 5000,
        });
        return false;
      } else if (this.appointment.maxPeople > 999) {
        this.notificationService.open('Appointment maximum persons that can join is 999', null, {
          duration: 5000,
        });
        return false;
      }
    } else if (this.appointment.endDate < this.appointment.beginDate) {
      this.notificationService.open('End Time of appointment has to be planned after begin time', null, {
        duration: 5000,
      });
      return false;
    } else if (this.appointment.description !== '') {
      if (this.appointment.description.length <= 4) {
        this.notificationService.open('Description has to be greater than 4 characters', null, {
          duration: 5000,
        });
        return false;
      } else if (this.appointment.description.length > 250) {
        this.notificationService.open('Description has to be smaller than 250 characters', null, {
          duration: 5000,
        });
        return false;
      }
    }
    return true;
  }
}
