using System;

namespace Transporter.Core.Utils;

public interface IDateTimeProvider
{
    DateTime Now { get; }
}
