import { Component, Input, OnInit } from '@angular/core';
import { DictionaryService, WordDto } from '../dictionary.service';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-word-details',
  templateUrl: './word-details.component.html',
  styleUrls: ['./word-details.component.css']
})
export class WordDetailsComponent implements OnInit {
  _word: string;
  wordDto: WordDto;

  spelling;

  @Input()
  set word(value: string) {
    if (!value) {
      return;
    }

    this.dictService.getWordDetails(value).subscribe(w => {
      this._word = value;
      this.wordDto = w;
      this.spelling = this.sanitize.bypassSecurityTrustHtml(w.spelling);
    });
  }

  constructor(private dictService: DictionaryService, private sanitize: DomSanitizer) { }

  ngOnInit() { }
}
