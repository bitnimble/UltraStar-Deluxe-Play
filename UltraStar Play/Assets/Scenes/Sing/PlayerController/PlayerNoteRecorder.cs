﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniInject;
using System.Linq;
using CSharpSynth.Wave;

// Disable warning about fields that are never assigned, their values are injected.
#pragma warning disable CS0649

// Takes the analyzed pitches from the PlayerPitchTracker and creates display events to draw recorded notes.
// For example, multiple beats next to each other can be considered as one note.
[RequireComponent(typeof(PlayerPitchTracker))]
public class PlayerNoteRecorder : MonoBehaviour, INeedInjection, IInjectionFinishedListener
{
    [Inject(searchMethod = SearchMethods.GetComponentInChildren)]
    public PlayerPitchTracker PlayerPitchTracker { get; private set; }

    private RecordedNote lastRecordedNote;
    private PlayerPitchTracker.BeatAnalyzedEvent lastBeatAnalyzedEvent;

    private Subject<RecordedNoteStartedEvent> recordedNoteStartedEventStream = new Subject<RecordedNoteStartedEvent>();
    public IObservable<RecordedNoteStartedEvent> RecordedNoteStartedEventStream
    {
        get
        {
            return recordedNoteStartedEventStream;
        }
    }

    private Subject<RecordedNoteContinuedEvent> recordedNoteContinuedEventStream = new Subject<RecordedNoteContinuedEvent>();
    public IObservable<RecordedNoteContinuedEvent> RecordedNoteContinuedEventStream
    {
        get
        {
            return recordedNoteContinuedEventStream;
        }
    }


    public void OnInjectionFinished()
    {
        PlayerPitchTracker.BeatAnalyzedEventStream
            .Subscribe(OnBeatAnalyzed);
    }

    private void OnBeatAnalyzed(PlayerPitchTracker.BeatAnalyzedEvent beatAnalyzedEvent)
    {
        Note analyzedNote = beatAnalyzedEvent.NoteAtBeat;
        if (lastRecordedNote != null
            && lastBeatAnalyzedEvent != null
            && lastBeatAnalyzedEvent.NoteAtBeat == analyzedNote
            && lastBeatAnalyzedEvent.RoundedMidiNote == beatAnalyzedEvent.RoundedMidiNote)
        {
            ContinueLastRecordedNote(beatAnalyzedEvent.Beat);
        }
        else if (beatAnalyzedEvent.PitchEvent != null)
        {
            StartNewRecordedNote(beatAnalyzedEvent.Beat, beatAnalyzedEvent.NoteAtBeat, beatAnalyzedEvent.PitchEvent.MidiNote, beatAnalyzedEvent.RoundedMidiNote);
        }
        else
        {
            lastRecordedNote = null;
        }

        lastBeatAnalyzedEvent = beatAnalyzedEvent;
    }

    private void ContinueLastRecordedNote(int analyzedBeat)
    {
        lastRecordedNote.EndBeat = analyzedBeat + 1;
        recordedNoteContinuedEventStream.OnNext(new RecordedNoteContinuedEvent(lastRecordedNote));
    }

    private void StartNewRecordedNote(int analyzedBeat, Note noteAtBeat, int recordedMidiNote, int roundedMidiNote)
    {
        RecordedNote newRecordedNote = new RecordedNote(recordedMidiNote, roundedMidiNote, analyzedBeat, analyzedBeat + 1, noteAtBeat);
        recordedNoteStartedEventStream.OnNext(new RecordedNoteStartedEvent(newRecordedNote));
        lastRecordedNote = newRecordedNote;
    }

    public class RecordedNoteStartedEvent
    {
        public RecordedNote RecordedNote { get; private set; }

        public RecordedNoteStartedEvent(RecordedNote recordedNote)
        {
            this.RecordedNote = recordedNote;
        }
    }

    public class RecordedNoteContinuedEvent
    {
        public RecordedNote RecordedNote { get; private set; }

        public RecordedNoteContinuedEvent(RecordedNote recordedNote)
        {
            this.RecordedNote = recordedNote;
        }
    }
}
