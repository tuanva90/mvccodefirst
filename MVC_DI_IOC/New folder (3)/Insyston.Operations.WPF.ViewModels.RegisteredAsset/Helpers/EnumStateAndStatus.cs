// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumStateAndStatus.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the EnumStateAndStatus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.RegisteredAsset.Helpers
{
    /// <summary>
    /// The enum state and status.
    /// </summary>
    public enum EnumStateAndStatus
    {
        InactiveIdle,
        ActiveIdle,
        ActiveReturned,
        ActiveLive,
        ActiveReserved,
        Terminated,
        None,
        Add,
    }
}
