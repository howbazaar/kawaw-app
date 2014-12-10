using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace System
{
    /// <summary>
    ///   Serialization Attribute for classes. 
    /// </summary>

    /// <remarks>
    ///   Use SerializableAttribute to mark classes that do not implement
    ///   the ISerializable interface but that want to be serialized.
    ///
    ///   Failing to do so will cause the system to throw an exception.
    ///
    ///   When a class is market with the SerializableAttribute, all the
    ///   fields are automatically serialized with the exception of those
    ///   that are tagged with the NonSerializedAttribute.
    ///
    ///   SerializableAttribute should only be used for classes that contain
    ///   simple data types that can be serialized and deserialized by the
    ///   runtime (typically you would use NonSerializedAttribute on data
    ///   that can be reconstructed at any point: like caches or precomputed
    ///   tables). 
    /// </remarks>

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct
        | AttributeTargets.Enum | AttributeTargets.Delegate,
        Inherited = false)]
    [ComVisible(true)]
    public sealed class SerializableAttribute : Attribute
    {
    }
}

namespace Kawaw
{
    [DataContract]
    public class RemoteUser 
    {
        // NOTE: probably want to store the cookies with the user so it gets persisted correctly.
        [DataMember(Name = "user")]
        private JSON.User _user;

        public RemoteUser()
        {
        }

        public RemoteUser(JSON.User user)
        {
            _user = user;
        }

        public string FullName {get { return _user.FullName; }}
    }
}