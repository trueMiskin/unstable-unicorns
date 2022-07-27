using System;

namespace UnstableUnicornCore {
    public class EndGameException : Exception {
        public EndGameException() {}

        public EndGameException(string? message) : base(message) {}
    }
}
