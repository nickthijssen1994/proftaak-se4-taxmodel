import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';


declare var $: any;
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent {
  constructor(private http: HttpClient) { }
}

