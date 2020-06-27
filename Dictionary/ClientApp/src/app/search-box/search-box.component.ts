import { Component, EventEmitter, HostListener, Input, OnInit, Output } from '@angular/core';
import { Subscription } from 'rxjs';
import { DictionaryService, SuggestionDto } from '../dictionary.service';

@Component({
  selector: 'app-search-box',
  templateUrl: './search-box.component.html',
  styleUrls: ['./search-box.component.css']
})
export class SearchBoxComponent implements OnInit {
  suggestionWords: SuggestionDto[] = [];

  isVisible = false;

  @Input()
  word = '';

  @Output()
  selectWord = new EventEmitter<string>();

  @Output()
  search = new EventEmitter<EnteringWord>();

  subscription: Subscription;

  constructor(private dictService: DictionaryService) { }

  ngOnInit() {
  }

  onEnterKeyup(e: KeyboardEvent) {
    this.isVisible = false;
    const searchingWord = (e.target as HTMLInputElement).value;

    this.search.emit({
      exactWord: (searchingWord || '').trim(),
      suggestionWord: this.suggestionWords.length ? this.suggestionWords[0].word : searchingWord
    });
  }

  onInput(e/*: InputEvent*/) {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }

    const searchingWord = (e.target as HTMLInputElement).value;
    if (!searchingWord) {
      this.suggestionWords = [];
      return;
    }

    this.subscription = this.dictService.searchWords(searchingWord).subscribe(ws => {
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

  onSearch(w: string) {
    this.search.emit({
      exactWord: (w || '').trim(),
      suggestionWord: w
    });
  }
}

export interface EnteringWord {
  exactWord: string;
  suggestionWord: string;
}

