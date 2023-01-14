import { Component, EventEmitter, HostListener, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Subscription } from 'rxjs';
import { DictionaryService, SuggestionDto } from '../dictionary.service';

@Component({
  selector: 'app-search-box',
  templateUrl: './search-box.component.html',
  styleUrls: ['./search-box.component.css']
})
export class SearchBoxComponent implements OnDestroy {
  private subscription: Subscription;

  suggestionWords: SuggestionDto[] = [];

  isSuggestionPanelVisible = false;

  @Input()
  word = '';

  @Output()
  selectWord = new EventEmitter<string>();

  @Output()
  search = new EventEmitter<EnteringWord>();

  constructor(private dictService: DictionaryService) { }

  ngOnDestroy(): void {
    this.unsubscribe();
  }

  onEnterKeyup(e: KeyboardEvent) {
    this.isSuggestionPanelVisible = false;
    const searchingWord = (e.target as HTMLInputElement).value;

    this.search.emit({
      exactWord: searchingWord,
      bestSuggestionWord: this.suggestionWords.length ? this.suggestionWords[0].word : ''
    });
  }

  onInput(e/*: InputEvent*/) {
    this.unsubscribe();

    this.suggestionWords = [];

    const searchingWord = (e.target as HTMLInputElement).value.trim();
    if (!searchingWord) {
      return;
    }

    this.subscription = this.dictService.searchWords(searchingWord).subscribe(ws => {
      this.isSuggestionPanelVisible = true;
      this.suggestionWords = ws;
    });
  }

  onSuggestionWordClick(sw: SuggestionDto) {
    this.isSuggestionPanelVisible = false;
    this.word = sw.word;
    this.selectWord.emit(sw.word);
  }

  // Hide the suggestion panel when user clicks anywhere
  @HostListener('window:click', ['$event'])
  onWindowClick(_e: MouseEvent) {
    this.isSuggestionPanelVisible = false;
  }

  onSearch(w: string) {
    this.search.emit({
      exactWord: w,
      bestSuggestionWord: ''
    });
  }

  onFocus(e: FocusEvent) {
    (e.target as HTMLInputElement).select();
  }

  private unsubscribe() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}

export interface EnteringWord {
  exactWord: string;
  bestSuggestionWord: string;
}
