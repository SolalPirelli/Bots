using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System;

namespace Bots.Quiz.Scoreboards
{
    public sealed class FileScoreboard : IQuizScoreboard
    {
        private static readonly Encoding FileEncoding = Encoding.UTF8;
        private static readonly char ScoreDelimiter = '\0';

        private readonly string _fileName;
        private readonly Dictionary<string, Entry> _values;

        public FileScoreboard( string fileName )
        {
            _fileName = fileName;
            _values = Load( _fileName );
        }

        public Task<long> IncreaseScoreAsync( string userId, string userName, long increment )
        {
            if( _values.ContainsKey( userId ) )
            {
                _values[userId].Update( userName, increment );
            }
            else
            {
                _values.Add( userId, new Entry( userName, increment ) );
            }

            Save( _fileName, _values );

            return Task.FromResult( _values[userId].Score );
        }

        public Task<Dictionary<string, long>> GetScoresByNameAsync()
        {
            return Task.FromResult( _values.ToDictionary( p => p.Value.Name, p => p.Value.Score ) );
        }


        private static Dictionary<string, Entry> Load( string fileName )
        {
            using( var stream = File.Open( fileName, FileMode.OpenOrCreate, FileAccess.Read ) )
            using( var reader = new StreamReader( stream, FileEncoding ) )
            {
                var values = new Dictionary<string, Entry>();

                for( var line = reader.ReadLine(); line != null; line = reader.ReadLine() )
                {
                    var split = line.Split( ScoreDelimiter );
                    values.Add( split[0], new Entry( split[1], long.Parse( split[2] ) ) );
                }

                return values;
            }
        }

        private static void Save( string fileName, Dictionary<string, Entry> values )
        {
            var lines = values.Select( p => p.Key + ScoreDelimiter + p.Value.Name + ScoreDelimiter + p.Value.Score );
            File.WriteAllLines( fileName, lines, FileEncoding );
        }

        private sealed class Entry
        {
            public string Name { get; private set; }
            public long Score { get; private set; }

            public Entry( string name, long score )
            {
                Name = name;
                Score = score;
            }

            public void Update( string name, long increment )
            {
                Name = name;
                Score += increment;
            }
        }
    }
}