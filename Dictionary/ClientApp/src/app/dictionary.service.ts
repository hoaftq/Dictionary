import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class DictionaryService {

  constructor(private http: HttpClient) { }

  getWordDetails(word: string): Observable<WordDto> {
    const url = `api/Dictionary/${word}`;
    return this.http.get<WordDto>(url);
  }

  searchWords(word: string): Observable<SuggestionDto[]> {
    const url = 'api/Dictionary/Search';
    const body = `word=${word}`;
    const options = { headers: new HttpHeaders().set('Content-Type', 'application/x-www-form-urlencoded; charset=utf-8') };
    return this.http.post<WordDto[]>(url, body, options).pipe(
      map(
        v => v.map(w => {
          const details: SuggestionDetailsDto[] = [];
          const sds = w.subDictionaries;
          if (sds && sds[0]) {
            const wcs = sds[0].wordClasses;
            if (wcs && wcs[0] && wcs[0].definitions && wcs[0].definitions[0]) {
              details.push({
                wordClass: wcs[0].name,
                definition: wcs[0].definitions[0].content
              });
            }
          }
          return {
            word: w.content,
            details: details
          };
        })
      )
    );
  }
}

export interface WordDto {
  content: string;
  spelling: string;
  spellingAudioUrl: string;
  subDictionaries: WordSubDictionaryDto[],
  wordForms: WordFormDto[],
  relativeWords: RelativeWordDto[]
}

export interface WordSubDictionaryDto {
  wordClasses: DictionaryWordClassDto[];
  phases: PhaseDto[];
}

export interface WordFormDto {
  formType: string;
  content: string;
}

export interface RelativeWordDto {
  isSynonym: boolean;
  wordClass: string;
  content: string;
}

export interface DictionaryWordClassDto {
  name: string;
  definitions: DefinitionDto[];
}

export interface PhaseDto {
  content: string;
  definitions: DefinitionDto[];
}

export interface DefinitionDto {
  content: string;
  usages: UsageDto[];
}

export interface UsageDto {
  sample: string;
  translation: string;
}

export interface SuggestionDto {
  word: string;
  details: SuggestionDetailsDto[]
}

export interface SuggestionDetailsDto {
  wordClass: string;
  definition: string;
}
