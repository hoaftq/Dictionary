import { Component, OnInit } from '@angular/core';
import { EnteringWord } from '../search-box/search-box.component';

@Component({
  selector: 'app-dictionary',
  templateUrl: './dictionary.component.html',
  styleUrls: ['./dictionary.component.css']
})
export class DictionaryComponent implements OnInit {

  word = '';

  constructor() { }

  ngOnInit() { }

  onEnterWord(e: EnteringWord) {
    if (e.firstSuggestion) {
      this.word = e.firstSuggestion;
    }
  }

  onSelectWord(w: string) {
    this.word = w;
  }

}
