using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class JobQueueService
    {
        private readonly Channel<string> _queue;

        public JobQueueService()
        {
            // Tạo hàng đợi không giới hạn (hoặc có thể giới hạn số lượng nếu muốn)
            _queue = Channel.CreateUnbounded<string>();
        }

        // Controller gọi hàm này để đẩy userId vào hàng đợi
        public async ValueTask QueueBackgroundWorkItemAsync(string userId)
        {
            await _queue.Writer.WriteAsync(userId);
        }

        // Worker gọi hàm này để lấy userId ra xử lý
        public async ValueTask<string> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
