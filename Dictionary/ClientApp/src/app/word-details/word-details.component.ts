import { Component, Input, OnDestroy } from '@angular/core';
import { DictionaryService, WordDto, ErrorDto } from '../dictionary.service';
import { DomSanitizer } from '@angular/platform-browser';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-word-details',
  templateUrl: './word-details.component.html',
  styleUrls: ['./word-details.component.css']
})
export class WordDetailsComponent implements OnDestroy {
  searchingWord: string;
  wordDto: WordDto;
  errorDto: ErrorDto;
  isSearching: boolean;

  subscription: Subscription;

  @Input()
  set word(value: string) {
    this.unsubscribe();

    this.searchingWord = value.trim();
    if (!this.searchingWord) {
      return;
    }

    // Reset status before searching
    this.isSearching = true;
    this.wordDto = null;
    this.errorDto = null;

    this.subscription = this.dictService.getWordDetails(encodeURIComponent(this.searchingWord)).subscribe(r => {
      this.wordDto = r.word;
      this.errorDto = r.error;

      this.isSearching = false;
    });
  }

  get spelling() {
    return this.sanitize.bypassSecurityTrustHtml(this.wordDto ? this.wordDto.spelling : '');
  }

  constructor(private dictService: DictionaryService, private sanitize: DomSanitizer) { }

  ngOnDestroy(): void {
    this.unsubscribe();
  }

  private unsubscribe() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
