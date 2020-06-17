import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DictionaryService {

  constructor() { }
}

export interface WordDto {
  content: string;
  spelling: string;
  spellingAudioUrl: string;
  subDirectories: WordSubDictionaryDto[],
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
