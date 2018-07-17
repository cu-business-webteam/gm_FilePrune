using System.Linq;
public static class Program {

	private sealed class Pair<S, T> {
		private readonly S myFirst;
		private readonly T mySecond;

		public Pair( S first, T second ) : base() {
			myFirst = first;
			mySecond = second;
		}

		public S First {
			get {
				return myFirst;
			}
		}
		public T Second {
			get {
				return mySecond;
			}
		}
	}


	[System.STAThread]
	public static System.Int32 Main( System.String[] args ) {
		if ( ( null == args ) || ( args.Length < 4 ) ) {
			WriteUsage();
			return 1;
		}
		var parsedSearchOption = false;
		System.IO.SearchOption searchOption;
		var searchOptionString = args[ 2 ];
		parsedSearchOption = System.Enum.TryParse<System.IO.SearchOption>( searchOptionString, true, out searchOption );
		var isDefined = parsedSearchOption && System.Enum.IsDefined( typeof( System.IO.SearchOption ), searchOption );
		if ( !isDefined ) {
			WriteUsage();
			return 1;
		}

		var timeSpanStringComponents = args[ 3 ].Split(
			new System.Char[ 1 ] { ':' }, 
			System.StringSplitOptions.RemoveEmptyEntries
		);
		var timeSpanComponentLen = timeSpanStringComponents.Length;
		if ( ( timeSpanComponentLen < 3 ) || ( 4 < timeSpanComponentLen ) ) {
			WriteUsage();
			return 1;
		}
		var timeSpan = ( 3 == timeSpanComponentLen )
			? new System.TimeSpan( 
				System.Int32.Parse( timeSpanStringComponents[ 0 ] ),
				System.Int32.Parse( timeSpanStringComponents[ 1 ] ),
				System.Int32.Parse( timeSpanStringComponents[ 2 ] )
			)
			: new System.TimeSpan( 
				System.Int32.Parse( timeSpanStringComponents[ 0 ] ),
				System.Int32.Parse( timeSpanStringComponents[ 1 ] ),
				System.Int32.Parse( timeSpanStringComponents[ 2 ] ),
				System.Int32.Parse( timeSpanStringComponents[ 3 ] )
			)
		;

		var path = args[ 0 ];
		var mask = args[ 1 ];

		var fileList = System.IO.Directory.EnumerateFiles( path, mask, searchOption ).Select(
			x => new Pair<System.String, System.IO.FileInfo>( x, new System.IO.FileInfo( x ) )
		).Where(
			x => x.Second.LastWriteTime < ( System.DateTime.Now - timeSpan )
		);
		foreach ( var x in fileList ) {
			System.Console.Out.WriteLine( x.First );
		}

		return 0;
	}

	private static void WriteUsage() {
		System.Console.Error.WriteLine( "No, no, no! Use it like this, Einstein!" );
		System.Console.Error.WriteLine( "FilePrune.exe path mask searchOption timeSpan" );
		System.Console.Error.WriteLine( "Where timeSpan takes the form of [d:]h:m:s, and components must be integers and may be negative." );
	}

}