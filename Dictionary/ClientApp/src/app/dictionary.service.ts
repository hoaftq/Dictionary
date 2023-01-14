import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class DictionaryService {

  constructor(private http: HttpClient) { }

  getWordDetails(word: string): Observable<Result> {
    const url = `api/Dictionary/${word}`;
    return this.http.get<WordDto>(url).pipe(
      map(word => ({ word })),
      catchError(err => of({
        error: {
          code: err.status,
          message: err.message
        }
      }))
    );
  }

  searchWords(word: string): Observable<SuggestionDto[]> {
    const url = 'api/Dictionary/Search';
    const body = `word=${word}`;
    const options = { headers: new HttpHeaders().set('Content-Type', 'application/x-www-form-urlencoded; charset=utf-8') };
    return this.http.post<SuggestionDto[]>(url, body, options);
  }
}

export interface Result {
  word?: WordDto;
  error?: ErrorDto;
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
  spelling: string;
  wordClass: string;
  definition: string;
}

export interface ErrorDto {
  code: number;
  message: string;
}
