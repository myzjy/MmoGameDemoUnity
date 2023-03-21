#if !BESTHTTP_DISABLE_SIGNALR_CORE
namespace BestHTTP.SignalRCore
{
    public static class HubConnectionExtensions
    {
        public
            static
            // ReSharper disable once UnusedTypeParameter
            UpStreamItemController<TResult> GetUpAndDownStreamController<TResult, T1>(
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
            // ReSharper disable once UnusedTypeParameter
            UpStreamItemController<TResult> GetUpAndDownStreamController<TResult, T1>(
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
            // ReSharper disable once UnusedTypeParameter
            // ReSharper disable once UnusedTypeParameter
            UpStreamItemController<TResult> GetUpAndDownStreamController<TResult, T1, T2>(
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
            // ReSharper disable once UnusedTypeParameter
            // ReSharper disable once UnusedTypeParameter
            UpStreamItemController<TResult> GetUpAndDownStreamController<TResult, T1, T2>(
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
            UpStreamItemController<TResult> GetUpAndDownStreamController<TResult, T1, T2, T3>(
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
            UpStreamItemController<TResult> GetUpAndDownStreamController<TResult, T1, T2, T3>(
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
            UpStreamItemController<TResult> GetUpAndDownStreamController<TResult, T1, T2, T3, T4>(
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
            UpStreamItemController<TResult> GetUpAndDownStreamController<TResult, T1, T2, T3, T4>(
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
            UpStreamItemController<TResult> GetUpAndDownStreamController<TResult, T1, T2, T3, T4, T5>(
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
            UpStreamItemController<TResult> GetUpAndDownStreamController<TResult, T1, T2, T3, T4, T5>(
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
            UpStreamItemController<TResult> GetUpStreamController<TResult, T1>(
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
            UpStreamItemController<TResult> GetUpStreamController<TResult, T1>(
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
            UpStreamItemController<TResult> GetUpStreamController<TResult, T1, T2>(
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
            UpStreamItemController<TResult> GetUpStreamController<TResult, T1, T2>(this HubConnection hub,
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
            UpStreamItemController<TResult> GetUpStreamController<TResult, T1, T2, T3>(
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
            UpStreamItemController<TResult> GetUpStreamController<TResult, T1, T2, T3>(
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
            UpStreamItemController<TResult> GetUpStreamController<TResult, T1, T2, T3, T4>(
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
            UpStreamItemController<TResult> GetUpStreamController<TResult, T1, T2, T3, T4>(
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
            UpStreamItemController<TResult> GetUpStreamController<TResult, T1, T2, T3, T4, T5>(
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
            UpStreamItemController<TResult> GetUpStreamController<TResult, T1, T2, T3, T4, T5>(
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
            void UploadParam<TResult, P1>(
                this UpStreamItemController<TResult> controller,
                P1 item)
        {
            controller.UploadParam(controller.StreamingIds[0], item);
        }

        public
            static
            void UploadParam<TResult, P1, P2>(
                this UpStreamItemController<TResult> controller,
                P1 param1,
                P2 param2)
        {
            controller.UploadParam(controller.StreamingIds[0], param1);
            controller.UploadParam(controller.StreamingIds[1], param2);
        }

        public
            static
            void UploadParam<TResult, P1, P2, P3>(
                this UpStreamItemController<TResult> controller,
                P1 param1,
                P2 param2,
                P3 param3)
        {
            controller.UploadParam(controller.StreamingIds[0], param1);
            controller.UploadParam(controller.StreamingIds[1], param2);
            controller.UploadParam(controller.StreamingIds[2], param3);
        }

        public
            static
            void UploadParam<TResult, P1, P2, P3, P4>(
                this UpStreamItemController<TResult> controller,
                P1 param1,
                P2 param2,
                P3 param3,
                P4 param4)
        {
            controller.UploadParam(controller.StreamingIds[0], param1);
            controller.UploadParam(controller.StreamingIds[1], param2);
            controller.UploadParam(controller.StreamingIds[2], param3);
            controller.UploadParam(controller.StreamingIds[3], param4);
        }

        public
            static
            void UploadParam<TResult, P1, P2, P3, P4, P5>(
                this UpStreamItemController<TResult> controller,
                P1 param1,
                P2 param2,
                P3 param3,
                P4 param4,
                P5 param5)
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