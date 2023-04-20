#if !BESTHTTP_DISABLE_SIGNALR_CORE
namespace BestHTTP.SignalRCore
{
    public static class HubConnectionExtensions
    {
        public
            static
            UpStreamItemController<TResult> GetUpAndDownStreamController8<TResult>(
                this HubConnection hub,
                string target)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 1,
                    downStream: true,
                    args: HubConnection.EmptyArgs);
        }

        public
            static
            UpStreamItemController<TResult> GetUpAndDownStreamController7<TResult>(
                this HubConnection hub,
                string target,
                params object[] args)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 1,
                    downStream: true,
                    args: args);
        }

        public
            static
            UpStreamItemController<TResult> GetUpAndDownStreamController6<TResult>(
                this HubConnection hub,
                string target)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 2,
                    downStream: true,
                    args: HubConnection.EmptyArgs);
        }

        public
            static
            UpStreamItemController<TResult> GetUpAndDownStreamController5<TResult>(
                this HubConnection hub, string target, params object[] args)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 2,
                    downStream: true,
                    args: args);
        }

        public
            static
            UpStreamItemController<TResult> GetUpAndDownStreamController4<TResult>(
                this HubConnection hub,
                string target)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 3,
                    downStream: true,
                    args: HubConnection.EmptyArgs);
        }

        public
            static
            UpStreamItemController<TResult> GetUpAndDownStreamController3<TResult>(
                this HubConnection hub,
                string target,
                params object[] args)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 3,
                    downStream: true,
                    args: args);
        }

        public
            static
            UpStreamItemController<TResult> GetUpAndDownStreamController2<TResult>(
                this HubConnection hub,
                string target)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 4,
                    downStream: true,
                    args: HubConnection.EmptyArgs);
        }

        public
            static
            UpStreamItemController<TResult> GetUpAndDownStreamController1<TResult>(
                this HubConnection hub,
                string target,
                params object[] args)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 4,
                    downStream: true,
                    args: args);
        }

        public
            static
            UpStreamItemController<TResult> GetUpAndDownStreamController<TResult>(
                this HubConnection hub,
                string target)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 5,
                    downStream: true,
                    args: HubConnection.EmptyArgs);
        }

        public
            static
            UpStreamItemController<TResult> GetUpAndDownStreamController<TResult>(
                this HubConnection hub,
                string target,
                params object[] args)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 5,
                    downStream: true,
                    args: args);
        }

        public
            static
            UpStreamItemController<TResult> GetUpStreamController8<TResult>(
                this HubConnection hub,
                string target)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 1,
                    downStream: false,
                    args: HubConnection.EmptyArgs);
        }

        public
            static
            UpStreamItemController<TResult> GetUpStreamController7<TResult>(
                this HubConnection hub,
                string target,
                params object[] args)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 1,
                    downStream: false,
                    args: args);
        }

        public
            static
            UpStreamItemController<TResult> GetUpStreamController6<TResult>(
                this HubConnection hub,
                string target)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 2,
                    downStream: false,
                    args: HubConnection.EmptyArgs);
        }

        public
            static
            UpStreamItemController<TResult> GetUpStreamController5<TResult>(this HubConnection hub,
                string target,
                params object[] args)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 2,
                    downStream: false,
                    args: args);
        }

        public
            static
            UpStreamItemController<TResult> GetUpStreamController4<TResult>(
                this HubConnection hub,
                string target)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 3,
                    downStream: false,
                    args: HubConnection.EmptyArgs);
        }

        public
            static
            UpStreamItemController<TResult> GetUpStreamController3<TResult>(
                this HubConnection hub,
                string target,
                params object[] args)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 3,
                    downStream: false,
                    args: args);
        }

        public
            static
            UpStreamItemController<TResult> GetUpStreamController2<TResult>(
                this HubConnection hub,
                string target)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 4,
                    downStream: false,
                    args: HubConnection.EmptyArgs);
        }

        public
            static
            UpStreamItemController<TResult> GetUpStreamController1<TResult>(
                this HubConnection hub,
                string target,
                params object[] args)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 4,
                    downStream: false,
                    args: args);
        }

        public
            static
            UpStreamItemController<TResult> GetUpStreamController<TResult>(
                this HubConnection hub,
                string target)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 5,
                    downStream: false,
                    args: HubConnection.EmptyArgs);
        }

        public
            static
            UpStreamItemController<TResult> GetUpStreamController<TResult>(
                this HubConnection hub,
                string target,
                params object[] args)
        {
            return hub.State != ConnectionStates.Connected
                ? null
                : hub.GetUpStreamController<TResult>(
                    target: target,
                    paramCount: 5,
                    downStream: false,
                    args: args);
        }
    }

    public static class UploadItemControllerExtensions
    {
        public
            static
            void UploadParam<TResult, TP1>(
                this UpStreamItemController<TResult> controller,
                TP1 item)
        {
            controller.UploadParam(controller.StreamingIds[0], item);
        }

        public
            static
            void UploadParam<TResult, TP1, TP2>(
                this UpStreamItemController<TResult> controller,
                TP1 param1,
                TP2 param2)
        {
            controller.UploadParam(controller.StreamingIds[0], param1);
            controller.UploadParam(controller.StreamingIds[1], param2);
        }

        public
            static
            void UploadParam<TResult, TP1, TP2, TP3>(
                this UpStreamItemController<TResult> controller,
                TP1 param1,
                TP2 param2,
                TP3 param3)
        {
            controller.UploadParam(controller.StreamingIds[0], param1);
            controller.UploadParam(controller.StreamingIds[1], param2);
            controller.UploadParam(controller.StreamingIds[2], param3);
        }

        public
            static
            void UploadParam<TResult, TP1, TP2, TP3, TP4>(
                this UpStreamItemController<TResult> controller,
                TP1 param1,
                TP2 param2,
                TP3 param3,
                TP4 param4)
        {
            controller.UploadParam(controller.StreamingIds[0], param1);
            controller.UploadParam(controller.StreamingIds[1], param2);
            controller.UploadParam(controller.StreamingIds[2], param3);
            controller.UploadParam(controller.StreamingIds[3], param4);
        }

        public
            static
            void UploadParam<TResult, TP1, TP2, TP3, TP4, TP5>(
                this UpStreamItemController<TResult> controller,
                TP1 param1,
                TP2 param2,
                TP3 param3,
                TP4 param4,
                TP5 param5)
        {
            controller.UploadParam(controller.StreamingIds[0], param1);
            controller.UploadParam(controller.StreamingIds[1], param2);
            controller.UploadParam(controller.StreamingIds[2], param3);
            controller.UploadParam(controller.StreamingIds[3], param4);
            controller.UploadParam(controller.StreamingIds[4], param5);
        }
    }
}

#endif