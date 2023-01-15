import { Component, EventEmitter, HostListener, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { DictionaryService, SuggestionDto } from '../dictionary.service';

@Component({
  selector: 'app-search-box',
  templateUrl: './search-box.component.html',
  styleUrls: ['./search-box.component.css']
})
export class SearchBoxComponent implements OnInit, OnDestroy {
  private serviceSubscription: Subscription;
  private inputSubscription: Subscription;

  inputSubject = new Subject<string>();

  suggestionWords: SuggestionDto[] = [];

  isSuggestionPanelVisible = false;

  @Input()
  word = '';

  @Output()
  selectWord = new EventEmitter<string>();

  @Output()
  search = new EventEmitter<EnteringWord>();

  constructor(private dictService: DictionaryService) { }

  ngOnInit(): void {
    this.inputSubscription = this.inputSubject.asObservable()
      .pipe(
        debounceTime(200)
      )
      .subscribe(value => {
        this.unsubscribeService();

        this.suggestionWords = [];

        const searchingWord = value.trim();
        if (!searchingWord) {
          return;
        }

        this.serviceSubscription = this.dictService.searchWords(searchingWord).subscribe(ws => {
          this.isSuggestionPanelVisible = true;
          this.suggestionWords = ws;
        });
      })
  }

  ngOnDestroy(): void {
    this.unsubscribeService();
    this.inputSubscription.unsubscribe();
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
    this.inputSubject.next((e.target as HTMLInputElement).value);
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

  private unsubscribeService() {
    if (this.serviceSubscription) {
      this.serviceSubscription.unsubscribe();
    }
  }
}

export interface EnteringWord {
  exactWord: string;
  bestSuggestionWord: string;
}
