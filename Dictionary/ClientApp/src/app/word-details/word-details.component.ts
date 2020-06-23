import { Component, Input, OnInit } from '@angular/core';
import { DictionaryService, WordDto, ErrorDto } from '../dictionary.service';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-word-details',
  templateUrl: './word-details.component.html',
  styleUrls: ['./word-details.component.css']
})
export class WordDetailsComponent implements OnInit {
  searchWord: string;
  wordDto: WordDto;
  errorDto: ErrorDto;
  isSearching: boolean;

  @Input()
  set word(value: string) {
    if (!value) {
      return;
    }

    // Reset status before searching
    this.isSearching = true;
    this.searchWord = value;
    this.wordDto = null;
    this.errorDto = null;

    this.dictService.getWordDetails(encodeURIComponent(value)).subscribe(w => {

      // Found a word
      if ((<WordDto>w).content) {
        this.wordDto = w as WordDto;
      } else {

        // An error occured
        this.errorDto = w as ErrorDto;
      }

      this.isSearching = false;
    });
  }

  get spelling() {
    return this.sanitize.bypassSecurityTrustHtml(this.wordDto ? this.wordDto.spelling : '');
  }

  constructor(private dictService: DictionaryService, private sanitize: DomSanitizer) { }

  ngOnInit() { }
}
