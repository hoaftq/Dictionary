import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { DictionaryComponent } from './dictionary/dictionary.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { SearchBoxComponent } from './search-box/search-box.component';
import { WordDetailsComponent } from './word-details/word-details.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    DictionaryComponent,
    SearchBoxComponent,
    WordDetailsComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: DictionaryComponent, pathMatch: 'full' }
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
