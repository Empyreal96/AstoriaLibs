using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
	public class WorkScheduler : IDisposable
	{
		private object lockObject = new object();

		private IList<IWork> works = new List<IWork>();

		private AutoResetEvent worksChangeEvent = new AutoResetEvent(initialState: false);

		private ManualResetEvent stopEvent = new ManualResetEvent(initialState: false);

		public void AddWorks(params IWork[] workList)
		{
			lock (lockObject)
			{
				AppendWorks(workList);
				worksChangeEvent.Set();
			}
		}

		public void AssignWorks(params IWork[] workList)
		{
			lock (lockObject)
			{
				works.Clear();
				AppendWorks(workList);
				worksChangeEvent.Set();
			}
		}

		public async Task RunAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			using (cancellationToken.Register(delegate
			{
				stopEvent.Set();
			}))
			{
				await Task.Factory.StartNew(delegate
				{
					Run();
				}, TaskCreationOptions.LongRunning);
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				worksChangeEvent.Dispose();
				stopEvent.Dispose();
			}
		}

		private void AppendWorks(params IWork[] workList)
		{
			if (workList == null || workList.Length <= 0)
			{
				return;
			}
			foreach (IWork work in workList)
			{
				if (work == null)
				{
					throw new ArgumentException("Input should not contain null object", "workList");
				}
				works.Add(work);
			}
		}

		private void Run()
		{
			while (true)
			{
				IList<WaitHandle> list = new List<WaitHandle>();
				list.Add(worksChangeEvent);
				list.Add(stopEvent);
				lock (lockObject)
				{
					foreach (IWork work in works)
					{
						list.Add(work.SignalHandle);
					}
				}
				int num = -1;
				try
				{
					num = WaitHandle.WaitAny(list.ToArray());
				}
				catch (ObjectDisposedException)
				{
					break;
				}
				WaitHandle waitHandle = list[num];
				if (waitHandle == stopEvent)
				{
					break;
				}
				if (waitHandle == worksChangeEvent)
				{
					continue;
				}
				IList<IWork> list2 = new List<IWork>();
				lock (lockObject)
				{
					foreach (IWork work2 in works)
					{
						if (work2.SignalHandle == waitHandle)
						{
							list2.Add(work2);
						}
					}
				}
				foreach (IWork item in list2)
				{
					item.DoWork();
				}
			}
		}
	}
}
