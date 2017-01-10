using Microsoft.AspNet.SignalR;

namespace RServer
{
    public class MoveShapeHub : Hub<IShapeClient>
    {
        public void UpdateModel(ShapeModel clientModel)
        {
            clientModel.LastUpdatedBy = Context.ConnectionId;
            // Update the shape model within our broadcaster
            Clients.AllExcept(clientModel.LastUpdatedBy).UpdateShape(clientModel);
        }
    }
}
