import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {environment} from '../../environments/environment';
import {LoginDto} from '../models/dtos/login-dto';
import {RegisterDto} from '../models/dtos/register-dto';
import {Router} from '@angular/router';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private headers = new HttpHeaders();

  httpOptions = {
    headers: this.headers,
  };

  private accountUrl = environment.apiUrl + '/account';  // URL to web api

  constructor(private http: HttpClient, private router: Router) {
    this.headers = this.headers.set('Access-Control-Allow-Origin', '*');
    this.headers = this.headers.set('Content-Type', 'application/json');
    this.headers = this.headers.set('Accept', 'application/json');
  }

  getByName(name: string): Observable<any>{
    const url = this.accountUrl + '/' + name;
    return this.http.get(url);
  }

  login(account: LoginDto) {
    return this.http.post<LoginDto>(this.accountUrl + '/login', account, this.httpOptions);
  }

  register(account: RegisterDto) {
    return this.http.post<RegisterDto>(this.accountUrl + '/register', account, this.httpOptions);
  }
}
