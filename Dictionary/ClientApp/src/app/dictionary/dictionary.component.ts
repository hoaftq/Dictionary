import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { EnteringWord } from '../search-box/search-box.component';

@Component({
  selector: 'app-dictionary',
  templateUrl: './dictionary.component.html',
  styleUrls: ['./dictionary.component.css']
})
export class DictionaryComponent implements OnInit, OnDestroy {
  searchingWord = '';
  subscription: Subscription;

  constructor(private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.subscription = this.route.paramMap.subscribe(ps => {
      const w = ps.get('word');
      this.searchingWord = w ? decodeURIComponent(w) : '';
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  // A word is entered in the search box
  onSearchWord(e: EnteringWord) {
    let wordToSearch: string;
    if (e.bestSuggestionWord
      && e.bestSuggestionWord.toLocaleLowerCase().startsWith(e.exactWord.trim().toLocaleLowerCase())) {
      wordToSearch = e.bestSuggestionWord;
    } else {
      wordToSearch = e.exactWord;
    }

    this.router.navigate([`/${encodeURIComponent(wordToSearch)}`]);
  }

  // A word is selected in the suggestion panel
  onSelectWord(w: string) {
    this.router.navigate([`/${encodeURIComponent(w)}`]);
  }
}
