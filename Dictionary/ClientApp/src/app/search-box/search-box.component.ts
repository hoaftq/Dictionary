import { Component, OnInit } from '@angular/core';
import { DictionaryService, WordDto, SuggestionDto } from '../dictionary.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-search-box',
  templateUrl: './search-box.component.html',
  styleUrls: ['./search-box.component.css']
})
export class SearchBoxComponent implements OnInit {

  suggestionWords: SuggestionDto[];

  constructor(private dictService: DictionaryService) { }

  ngOnInit() {
  }

  onKeyup(e: KeyboardEvent) {
    const searchingWord = (e.target as HTMLInputElement).value;
    this.dictService.searchWords(searchingWord).subscribe(ws => {
      this.suggestionWords = ws;
    });
  }
}

