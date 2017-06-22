using System;
using System.Diagnostics;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PerformWork : MonoBehaviour
{
    public Text TextFieldToUpdate;
    private ReactiveProperty<long> _reactiveHandle;
    private ReactiveProperty<long> _bulkReactiveHandle;
    Stopwatch sw = new Stopwatch();

    /// <summary>
    /// Instantiates collections and subscribes callback functions
    /// </summary>
    private void Awake()
    {
        _reactiveHandle = new ReactiveProperty<long>(); // simple tasks
        _bulkReactiveHandle = new ReactiveProperty<long>(); // 1000000 iterations

        _reactiveHandle
            .ObserveOn(Scheduler.MainThread)
            .Subscribe((val) =>
            {
                LongOperation(val.ToString());
            });

        _bulkReactiveHandle
            .ObserveOn(Scheduler.MainThread)
            .Subscribe((val) =>
            {
                if (val == 0) { sw.Reset(); sw.Start(); }

                if (TextFieldToUpdate != null)
                    TextFieldToUpdate.text = val.ToString() + " item took (ms): " + sw.ElapsedMilliseconds;

                if (val == 999999) sw.Stop();
            });
    }

    /// <summary>
    /// Updates our Reactive Property with a new value to kick off the subscriber
    /// </summary>
    public void DoSingleItemWork_Click()
    {
        _reactiveHandle.Value = DateTime.Now.Ticks;
    }

    /// <summary>
    /// Updates our Reactive Property 3 times with a new value to kick off the subscriber
    /// </summary>
    public void DoMultipleItemsWork_Click()
    {
        _reactiveHandle.Value = DateTime.Now.Ticks;
        _reactiveHandle.Value = DateTime.Now.Ticks + 1;
        _reactiveHandle.Value = DateTime.Now.Ticks + 2;
    }


    /// <summary>
    /// Updates the bulk reactive property 1000000 times
    /// </summary>
    public void DoBulkWork_Click()
    {
        for (int i = 0; i < 1000000; i++)
        {
            _bulkReactiveHandle.Value = i;
        }
    }

    /// <summary>
    /// Performs a Thread.Sleep for 3 seconds
    /// </summary>
    /// <param name="delayText">Text to display</param>
    private void LongOperation(string delayText)
    {
        System.Threading.Thread.Sleep(3000); // main thread blocker
        // Update UI - proves we work on the main thread
        TextFieldToUpdate.text = delayText + " blocking work complete. " + System.DateTime.Now.ToString();
    }
}
