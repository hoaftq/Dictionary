import { Component, OnInit, Input } from '@angular/core';
import { WordDto, DictionaryService } from '../dictionary.service';

@Component({
  selector: 'app-word-details',
  templateUrl: './word-details.component.html',
  styleUrls: ['./word-details.component.css']
})
export class WordDetailsComponent implements OnInit {

  @Input()
  word: string;

  wordDto: WordDto;

  constructor(private dictService: DictionaryService) { }

  ngOnInit() {
    this.dictService.getWordDetails(this.word).subscribe(w => {
      this.wordDto = w;
      //alert(JSON.stringify(w));
    });
  }
}
