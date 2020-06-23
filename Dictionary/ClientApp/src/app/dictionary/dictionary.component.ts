import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EnteringWord } from '../search-box/search-box.component';

@Component({
  selector: 'app-dictionary',
  templateUrl: './dictionary.component.html',
  styleUrls: ['./dictionary.component.css']
})
export class DictionaryComponent implements OnInit {
  word = '';

  constructor(private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.route.paramMap.subscribe(ps => {
      const w = ps.get('word');
      this.word = w ? decodeURIComponent(w) : w;
    });
  }

  // A word is entered in the search box
  onSearchWord(e: EnteringWord) {
    if (e.suggestionWord) {
      this.router.navigate([`/${encodeURIComponent(e.suggestionWord)}`]);
    }
  }

  // A word is selected in the suggestion panel
  onSelectWord(w: string) {
    this.router.navigate([`/${encodeURIComponent(w)}`]);
  }
}
