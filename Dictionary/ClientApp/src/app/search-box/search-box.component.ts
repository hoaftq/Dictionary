import { Component, EventEmitter, HostListener, OnInit, Output } from '@angular/core';
import { DictionaryService, SuggestionDto } from '../dictionary.service';

@Component({
  selector: 'app-search-box',
  templateUrl: './search-box.component.html',
  styleUrls: ['./search-box.component.css']
})
export class SearchBoxComponent implements OnInit {
  suggestionWords: SuggestionDto[] = [];

  isVisible = false;

  word = '';

  @Output()
  selectWord = new EventEmitter<string>();

  @Output()
  enterWord = new EventEmitter<EnteringWord>();

  constructor(private dictService: DictionaryService) { }

  ngOnInit() {
  }

  onEnterKeyup(e: KeyboardEvent) {
    this.isVisible = false;
    const searchingWord = (e.target as HTMLInputElement).value;
    this.enterWord.emit({
      word: searchingWord,
      firstSuggestion: this.suggestionWords.length ? this.suggestionWords[0].word : ''
    });
  }

  onInput(e/*: InputEvent*/) {
    const searchingWord = (e.target as HTMLInputElement).value;
    if (!searchingWord) {
      this.suggestionWords = [];
      return;
    }

    this.dictService.searchWords(searchingWord).subscribe(ws => {
      this.isVisible = true;
      this.suggestionWords = ws;
    });
  }

  onSuggestionWordClick(sw: SuggestionDto) {
    this.isVisible = false;
    this.word = sw.word;
    this.selectWord.emit(sw.word);
  }

  // Hide the suggestion panel when user clicks anywhere
  @HostListener('window:click', ['$event'])
  onWindowClick(_e: MouseEvent) {
    this.isVisible = false;
  }
}

export interface EnteringWord {
  word: string;
  firstSuggestion: string;
}

