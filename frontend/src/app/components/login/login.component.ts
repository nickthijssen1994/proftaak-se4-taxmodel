import { Component, OnInit } from '@angular/core';
import {LoginDto} from '../../models/LoginDto';
import {AccountService} from '../../services/account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  account: LoginDto = {
    name: '',
    password: ''
};

  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
  }

  login(account: LoginDto): void {
    this.accountService.login(account).subscribe();
    console.log(account);
    console.log('logged in');
  }
}
