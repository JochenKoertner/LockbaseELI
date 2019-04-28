
using System;

namespace Lockbase.CoreDomain.ValueObjects  {

    public class Statement {
        public Statement(string statement)
        {
            int index = statement.IndexOf(',');
            Head = statement.Substring(0,index);
            Tail = statement.Substring(index+1);
        }

        public string Head { get; private set; }
        public string Tail { get; private set; }
    }
}