﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Azure.Gaming.AgentInterfaces
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using ProtoBuf;

    /// <summary>
    /// <devnote> If the property names are changed in this class, make sure to reflect the change in the SessionHostHeartbeatInfoTests.</devnote> 
    /// </summary>
    [ProtoContract]
    public class SessionHostHeartbeatInfo
    {
        /// <summary>
        /// The current game state. For example - StandingBy, Active etc.
        /// </summary>
        [ProtoMember(1)]
        public SessionHostStatus CurrentGameState { get; set; }

        /// <summary>
        /// The number of milliseconds to wait before sending the next heartbeat.
        /// </summary>
        [ReadOnly(true)]
        [ProtoMember(2)]
        public int? NextHeartbeatIntervalMs { get; set; }

        [ReadOnly(true)]
        [ProtoMember(3)]
        public Operation? Operation { get; set; }

        /// <summary>
        /// The game host's current health.
        /// </summary>
        [ProtoMember(4)]
        public SessionHostHealth CurrentGameHealth { get; set; }

        /// <summary>
        /// List of players connected to the game host.
        /// </summary>
        [ProtoMember(5)]
        public List<ConnectedPlayer> CurrentPlayers { get; set; }

        /// <summary>
        /// The time at which the <see cref="CurrentGameState"/> had last changed.
        /// </summary>
        [ProtoMember(6)]
        [CompatibilityLevel(CompatibilityLevel.Level240)]
        public DateTime? LastStateTransitionTimeUtc { get; set; }

        /// <summary>
        /// The configuration sent down to the game host from Control Plane.
        /// </summary>
        [ProtoMember(7)]
        public SessionConfig SessionConfig { get; set; }

        /// <summary>
        /// The port mappings used by thsi session host.
        /// </summary>
        [ProtoMember(8)]
        public List<PortMapping> PortMappings { get; set; }

        /// <summary>
        /// The next scheduled maintenance time from Azure, in UTC.
        /// </summary>
        [ProtoMember(9)]
        [CompatibilityLevel(CompatibilityLevel.Level240)] // Level240 is the same as DataFormat.WellKnown => 
        // '.google.protobuf.Timestamp' for DateTime '.google.protobuf.Duration' for TimeSpan
        public DateTime? NextScheduledMaintenanceUtc { get; set; }

        /// <summary>
        /// Used by some legacy games such as Forza 5 for security handshake with the game client.
        /// </summary>
        [ProtoMember(10)]
        public string SecureDeviceAddress { get; set; }

        /// <summary>
        /// Identifies the title, deployment and region for this particular session host.
        /// This could differ among the session hosts running on a VM that is running multiple deployments.
        /// </summary>
        [ProtoMember(11)]
        public string AssignmentId { get; set; }

        /// <summary>
        /// The state of the crash dump (if any) on this session host.
        /// </summary>
        [ProtoMember(12)]
        public CrashDumpState CrashDumpState { get; set; }

        /// <summary>
        /// The state of the crash dump (if any) on this session host.
        /// </summary>
        [ProtoMember(13)]
        public bool ContainsProfilingOutput { get; set; }

        public bool IsStateSame(SessionHostHeartbeatInfo other)
        {
            // TODO : Consider adding a check for the current players elements as well. 
            // A player might have left and a new player joined in the same heartbeat which is a different heartbeat.
            // NOTE : If other properties are added here, make sure to add them to the equals list in the SessionHostHeartbeatInfoTests.
            return (CurrentGameHealth == other?.CurrentGameHealth
                && CurrentGameState == other?.CurrentGameState
                && CurrentPlayers?.Count == other?.CurrentPlayers?.Count);
        }

        /// <summary>
        /// Creates a shallow copy of the object. Useful when we want to copy existing fields and modify only a couple of them for a new object.
        /// </summary>
        /// <returns></returns>
        public SessionHostHeartbeatInfo ShallowCopy()
        {
            return (SessionHostHeartbeatInfo)MemberwiseClone();
        }
    }

    [ProtoContract]
    public class Port
    {
        /// <summary>
        /// The port name
        /// </summary>
        [Required]
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <summary>
        /// Port Number
        /// </summary>
        [Required]
        [ProtoMember(2)]
        public int Number { get; set; }

        /// <summary>
        /// The port protocol
        /// </summary>
        [Required]
        [ProtoMember(3)]
        public string Protocol { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="port"></param>
        /// <param name="protocol"></param>
        public Port(string name, int port, string protocol)
        {
            Name = name;
            Number = port;
            Protocol = protocol;
        }

        public Port() { }
    }

    // Explicitly declaring which member is the VM port, and which is the game port
    [ProtoContract]
    public class PortMapping
    {
        [Required]
        [ProtoMember(1)]
        public int PublicPort { get; set; }

        [Required]
        [ProtoMember(2)]
        public int NodePort { get; set; }

        [Required]
        [ProtoMember(3)]
        public Port GamePort { get; set; }
    }

    [ProtoContract]
    public class ConnectedPlayer
    {
        /// <summary>
        /// Gamer tag to identify this player
        /// </summary>
        [ProtoMember(1)]
        public string PlayerId { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    [ProtoContract]
    public enum CrashDumpState
    {
        [ProtoEnum]
        None,
        [ProtoEnum]
        Present,
        [ProtoEnum]
        Throttled,
    }
}
