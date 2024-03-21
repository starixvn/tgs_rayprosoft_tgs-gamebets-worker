using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.GameBets.Worker.Core.Models;

namespace Tgs.GameBets.Worker.Core;
public interface IGameWagerStatCloseRepository
{
	ValueTask AddAsync(GameWagerStatCloseAdd source, CancellationToken cancellationToken = default);
}