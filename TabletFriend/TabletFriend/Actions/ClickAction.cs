using System.Threading.Tasks;
using WindowsInput.Events;

namespace TabletFriend.Actions
{
	public class ClickAction : ButtonAction
	{
		private readonly string _coordinates;

		public ClickAction(string cmd)
		{
			_coordinates = cmd;
		}

		public async override Task Invoke()
		{
			// TODO: add error handling for parameter parsing
			var xy = _coordinates.Split(',');
			var x = xy[0];
			var y = xy[1];

			var sim = new EventBuilder();
			sim.MoveTo(int.Parse(x), int.Parse(y)).Click(ButtonCode.Left);
			await sim.Invoke();
		}
	}
}
