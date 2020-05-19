import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable, of} from 'rxjs';
import {Appointment} from '../models/Appointment';
import {environment} from '../../environments/environment';
import {UpdateRegistrationDto} from '../models/updateRegistrationDto';

@Injectable({
  providedIn: 'root'
})
export class UpdateRegistrationService {

  private headers = new HttpHeaders();

  httpOptions = {
    headers: this.headers,
  };

  private appointmentsUrl = environment.apiUrl + '/appointment';  // URL to web api

  constructor(private http: HttpClient) {
    this.headers = this.headers.set('Access-Control-Allow-Origin', '*');
    this.headers = this.headers.set('Content-Type', 'application/json');
    this.headers = this.headers.set('Accept', 'application/json');
  }

 subscribe(): void {
    // return this.http.post<Appointment[]>(this.appointmentsUrl);
  }

  unsubscribe(updateRegistrationDto: UpdateRegistrationDto): void {
    const url = `${this.appointmentsUrl}/unsubscribe/`;
    const header: HttpHeaders = new HttpHeaders()
      .append('Content-Type', 'application/json; charset=UTF-8');
    const httpOptions = {
      headers: header,
      body: { updateRegistrationDto: UpdateRegistrationDto }
    };
    this.http.delete<UpdateRegistrationDto>(url, httpOptions);
  }
}
