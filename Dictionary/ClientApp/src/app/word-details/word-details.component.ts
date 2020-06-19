import { Component, Input, OnInit } from '@angular/core';
import { DictionaryService, WordDto } from '../dictionary.service';

@Component({
  selector: 'app-word-details',
  templateUrl: './word-details.component.html',
  styleUrls: ['./word-details.component.css']
})
export class WordDetailsComponent implements OnInit {

  wordDto: WordDto;

  @Input()
  set word(value: string) {
    if (!value) {
      return;
    }

    this.dictService.getWordDetails(value).subscribe(w => {
      this.wordDto = w;
    });
  }

  constructor(private dictService: DictionaryService) { }

  ngOnInit() { }
}
