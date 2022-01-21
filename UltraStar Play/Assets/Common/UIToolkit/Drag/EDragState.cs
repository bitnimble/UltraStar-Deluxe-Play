﻿public enum EDragState
{
    /**
     * No pointer down yet.
     */
    WaitingForPointerDown,

    /**
     * Pointer is down but not moved over threshold distance yet.
     */
    ReadyForDrag,

    /**
     * Pointer is down and moved over threshold distance. Still dragging.
     */
    Dragging,

    /**
     * CancelDrag was called since last PointerDownEvent.
     */
    IgnoreDrag
}
