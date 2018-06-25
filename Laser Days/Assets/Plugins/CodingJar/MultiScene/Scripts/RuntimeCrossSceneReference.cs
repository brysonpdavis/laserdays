using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine.Serialization;

namespace CodingJar.MultiScene
{
	public class ResolveException : System.Exception
	{
		public ResolveException( string message ) : base( message ) {}
	}

	[System.Serializable]
	public struct GenericData
	{
		public Object	@object;
		public string	@string;

		public GenericData( Object obj ) { @object = obj; @string = null; }
		public GenericData( string str ) { @object = null; @string = str; }

		public static implicit operator GenericData( Object obj ) { return new GenericData( obj ); }
		public static implicit operator GenericData( string str ) { return new GenericData( str ); }
	}

	[System.Serializable]
    public struct RuntimeCrossSceneReference
    {
        // From which UniqueObject.PropertyPath?
		[SerializeField]					Object			_sourceObject;

		[FormerlySerializedAs("_fromField")]
		[SerializeField]					string			_sourceField;

		[FormerlySerializedAs("_fromObject")]
		[SerializeField, HideInInspector]	UniqueObject	DEPRECATED_fromObject;

        // Which UniqueObject are we referencing?
        [SerializeField]	UniqueObject	_toObject;

		// New ability to save any data with this cross-scene reference for generic resolving
		[System.Serializable]
		struct GenericDataBundle
		{
			public List<GenericData>	data;
		}

		[SerializeField]	GenericDataBundle	_data;

		public RuntimeCrossSceneReference( Object from, string fromField, UniqueObject to, List<GenericData> data )
		{
			DEPRECATED_fromObject = new UniqueObject();

			_sourceObject = from;
			_sourceField = fromField;
			_toObject = to;
			_data = new GenericDataBundle() { data = data };

			_toObjectCached = null;
		}

		public	Object	fromObject
		{
			get
			{
				// Update this object from old data
				if ( !_sourceObject && DEPRECATED_fromObject.scene.IsValid() )
				{
					_sourceObject = DEPRECATED_fromObject.Resolve();
					DEPRECATED_fromObject = new UniqueObject();
				}

				return _sourceObject;
			}
		}

		private	Object	_toObjectCached;
		public	Object	toObject
		{
			get
			{
				if ( !_toObjectCached )
					_toObjectCached = _toObject.Resolve();

				return _toObjectCached;
			}
		}

		public AmsSceneReference	DEPRECATED_fromScene
		{
			set
			{
				if ( !_sourceObject && DEPRECATED_fromObject.scene.IsValid() )
				{
					DEPRECATED_fromObject.scene = value;
				}
			}
		}

		public string sourceField
		{
			get { return _sourceField; }
		}

		public List<GenericData> data
		{
			get { return _data.data; }
		}

		public AmsSceneReference	toScene
		{
			get { return _toObject.scene;	}
			set { _toObject.scene = value;	}
		}

        public override string ToString()
        {
            return string.Format( "{0}.{1} => {2}", _sourceObject ? _sourceObject.ToString() : "(null)", _sourceField, _toObject );
        }

		public bool IsSameSource( RuntimeCrossSceneReference other )
		{
			try
			{
				return (this.fromObject == other.fromObject) && (this._sourceField == other._sourceField);
			}
			catch ( System.Exception ex )
			{
				AmsDebug.Log( null, "IsSameSource: Could not compare: {0} and {1}: {2}", ToString(), other, ex );
			}

			return false;
		}
	} // struct 
} // namespace 