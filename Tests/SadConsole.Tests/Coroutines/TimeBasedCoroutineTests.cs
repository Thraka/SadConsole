using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadConsole.Coroutine;

namespace SadConsole.Tests.Coroutines;

[TestClass]
public class TimeBasedCoroutineTests
{

    [TestMethod]
    public void TestTimerBasedCoroutine() {
        int counter = 0;

        IEnumerator<Wait> OnTimeTickCodeExecuted() {
            counter++;
            yield return new Wait(0.1d);
            counter++;
        }

        ActiveCoroutine cr = CoroutineHandler.Start(OnTimeTickCodeExecuted());
        Assert.AreEqual(1, counter, "instruction before yield is not executed.");
        Assert.AreEqual(string.Empty, cr.Name, "Incorrect default name found");
        Assert.AreEqual(0, cr.Priority, "Default priority is not minimum");
        for (int i = 0; i < 5; i++)
            CoroutineHandler.Tick(1);
        Assert.AreEqual(2, counter, "instruction after yield is not executed.");
        Assert.IsTrue(cr.IsFinished, "Incorrect IsFinished value.");
        Assert.IsFalse(cr.WasCanceled, "Incorrect IsCanceled value.");
        Assert.AreEqual(2, cr.MoveNextCount, "Incorrect MoveNextCount value.");
    }

    [TestMethod]
    public void TestCoroutineReturningWeirdYields() {
        int counter = 0;

        IEnumerator<Wait> OnTimeTickNeverReturnYield() {
            counter++; // 1
            // condition that's expected to be false
            if (counter == 100)
                yield return new Wait(0.1d);
            counter++; // 2
        }

        IEnumerator<Wait> OnTimeTickYieldBreak() {
            counter++; // 3
            yield break;
        }

        ActiveCoroutine[] cr = [
            CoroutineHandler.Start(OnTimeTickNeverReturnYield()),
            CoroutineHandler.Start(OnTimeTickYieldBreak())];

        for (int i = 0; i < 5; i++)
            CoroutineHandler.Tick(1);

        Assert.AreEqual(3, counter, "Incorrect counter value.");
        for (int i = 0; i < cr.Length; i++) {
            Assert.IsTrue(cr[i].IsFinished, $"Incorrect IsFinished value on index {i}.");
            Assert.IsFalse(cr[i].WasCanceled, $"Incorrect IsCanceled value on index {i}");
            Assert.AreEqual(1, cr[i].MoveNextCount, $"Incorrect MoveNextCount value on index {i}");
        }
    }

    [TestMethod]
    public void TestCoroutineReturningDefaultYield() {
        int counter = 0;

        IEnumerator<Wait> OnTimeTickYieldDefault() {
            counter++; // 1
            yield return default;
            counter++; // 2
        }

        ActiveCoroutine cr = CoroutineHandler.Start(OnTimeTickYieldDefault());
        for (int i = 0; i < 5; i++)
            CoroutineHandler.Tick(1);

        Assert.AreEqual(2, counter, "Incorrect counter value.");
        Assert.IsTrue(cr.IsFinished, "Incorrect IsFinished value.");
        Assert.IsFalse(cr.WasCanceled, "Incorrect IsCanceled value.");
        Assert.AreEqual(2, cr.MoveNextCount, "Incorrect MoveNextCount value.");
    }

    [TestMethod]
    public void TestInfiniteCoroutineNeverFinishesUnlessCanceled() {
        int counter = 0;

        IEnumerator<Wait> OnTimerTickInfinite() {
            while (true) {
                counter++;
                yield return new Wait(1);
            }
        }

        void SetCounterToUnreachableValue(ActiveCoroutine coroutine) {
            counter = -100;
        }

        ActiveCoroutine cr = CoroutineHandler.Start(OnTimerTickInfinite());
        cr.OnFinished += SetCounterToUnreachableValue;
        for (int i = 0; i < 50; i++)
            CoroutineHandler.Tick(1);

        Assert.AreEqual(51, counter, "Incorrect counter value.");
        Assert.IsFalse(cr.IsFinished, "Incorrect IsFinished value.");
        Assert.IsFalse(cr.WasCanceled, "Incorrect IsCanceled value.");
        Assert.AreEqual(51, cr.MoveNextCount, "Incorrect MoveNextCount value.");

        cr.Cancel();
        Assert.IsTrue(cr.WasCanceled, "Incorrect IsCanceled value after canceling.");
        Assert.AreEqual(-100, counter, "OnFinished event not triggered when canceled.");
        Assert.AreEqual(51, cr.MoveNextCount, "Incorrect MoveNextCount value.");
        Assert.IsTrue(cr.IsFinished, "Incorrect IsFinished value.");
    }

    [TestMethod]
    public void TestOnFinishedEventExecuted() {
        int counter = 0;

        IEnumerator<Wait> OnTimeTick() {
            counter++;
            yield return new Wait(0.1d);
        }

        void SetCounterToUnreachableValue(ActiveCoroutine coroutine) {
            counter = -100;
        }

        ActiveCoroutine cr = CoroutineHandler.Start(OnTimeTick());
        cr.OnFinished += SetCounterToUnreachableValue;
        CoroutineHandler.Tick(50);
        Assert.AreEqual(-100, counter, "Incorrect counter value.");
    }

    [TestMethod]
    public void TestNestedCoroutine() {
        int counterAlwaysRunning = 0;

        IEnumerator<Wait> AlwaysRunning() {
            while (true) {
                yield return new Wait(1);
                counterAlwaysRunning++;
            }
        }

        int counterChild = 0;

        IEnumerator<Wait> Child() {
            yield return new Wait(1);
            counterChild++;
        }

        int counterParent = 0;

        IEnumerator<Wait> Parent() {
            yield return new Wait(1);
            counterParent++;
            // OnFinish I will start child.
        }

        int counterGrandParent = 0;

        IEnumerator<Wait> GrandParent() {
            yield return new Wait(1);
            counterGrandParent++;
            // Nested corotuine starting.
            ActiveCoroutine p = CoroutineHandler.Start(Parent());
            // Nested corotuine starting in OnFinished.
            p.OnFinished += _ => CoroutineHandler.Start(Child());
        }

        ActiveCoroutine always = CoroutineHandler.Start(AlwaysRunning());
        CoroutineHandler.Start(GrandParent());
        Assert.AreEqual(0, counterAlwaysRunning, "Always running counter is invalid at time 0.");
        Assert.AreEqual(0, counterGrandParent, "Grand Parent counter is invalid at time 0.");
        Assert.AreEqual(0, counterParent, "Parent counter is invalid at time 0.");
        Assert.AreEqual(0, counterChild, "Child counter is invalid at time 0.");
        CoroutineHandler.Tick(1);
        Assert.AreEqual(1, counterAlwaysRunning, "Always running counter is invalid at time 1.");
        Assert.AreEqual(1, counterGrandParent, "Grand Parent counter is invalid at time 1.");
        Assert.AreEqual(0, counterParent, "Parent counter is invalid at time 1.");
        Assert.AreEqual(0, counterChild, "Child counter is invalid at time 1.");
        CoroutineHandler.Tick(1);
        Assert.AreEqual(2, counterAlwaysRunning, "Always running counter is invalid at time 2.");
        Assert.AreEqual(1, counterGrandParent, "Grand Parent counter is invalid at time 2.");
        Assert.AreEqual(1, counterParent, "Parent counter is invalid at time 2.");
        Assert.AreEqual(0, counterChild, "Child counter is invalid at time 2.");
        CoroutineHandler.Tick(1);
        Assert.AreEqual(3, counterAlwaysRunning, "Always running counter is invalid at time 3.");
        Assert.AreEqual(1, counterGrandParent, "Grand Parent counter is invalid at time 3.");
        Assert.AreEqual(1, counterParent, "Parent counter is invalid at time 3.");
        Assert.AreEqual(1, counterChild, "Child counter is invalid at time 3.");
        CoroutineHandler.Tick(1);
        Assert.AreEqual(4, counterAlwaysRunning, "Always running counter is invalid at time 4.");
        Assert.AreEqual(1, counterGrandParent, "Grand Parent counter is invalid at time 4.");
        Assert.AreEqual(1, counterParent, "Parent counter is invalid at time 4.");
        Assert.AreEqual(1, counterChild, "Child counter is invalid at time 4.");
        always.Cancel();
    }

    [TestMethod]
    public void TestPriority() {
        int counterShouldExecuteBefore0 = 0;

        IEnumerator<Wait> ShouldExecuteBefore0() {
            while (true) {
                yield return new Wait(1);
                counterShouldExecuteBefore0++;
            }
        }

        int counterShouldExecuteBefore1 = 0;

        IEnumerator<Wait> ShouldExecuteBefore1() {
            while (true) {
                yield return new Wait(1);
                counterShouldExecuteBefore1++;
            }
        }

        int counterShouldExecuteAfter = 0;

        IEnumerator<Wait> ShouldExecuteAfter() {
            while (true) {
                yield return new Wait(1);
                if (counterShouldExecuteBefore0 == 1 &&
                    counterShouldExecuteBefore1 == 1) {
                    counterShouldExecuteAfter++;
                }
            }
        }

        int counterShouldExecuteFinally = 0;

        IEnumerator<Wait> ShouldExecuteFinally() {
            while (true) {
                yield return new Wait(1);
                if (counterShouldExecuteAfter > 0) {
                    counterShouldExecuteFinally++;
                }
            }
        }

        const int highPriority = int.MaxValue;
        ActiveCoroutine before1 = CoroutineHandler.Start(ShouldExecuteBefore1(), priority: highPriority);
        ActiveCoroutine after = CoroutineHandler.Start(ShouldExecuteAfter());
        ActiveCoroutine before0 = CoroutineHandler.Start(ShouldExecuteBefore0(), priority: highPriority);
        ActiveCoroutine @finally = CoroutineHandler.Start(ShouldExecuteFinally(), priority: -1);
        CoroutineHandler.Tick(10);
        Assert.AreEqual(1, counterShouldExecuteAfter, $"ShouldExecuteAfter counter  {counterShouldExecuteAfter} is invalid.");
        Assert.AreEqual(1, counterShouldExecuteFinally, $"ShouldExecuteFinally counter  {counterShouldExecuteFinally} is invalid.");

        before1.Cancel();
        after.Cancel();
        before0.Cancel();
        @finally.Cancel();
    }

    [TestMethod]
    public void TestTimeBasedCoroutineIsAccurate() {
        int counter0 = 0;

        IEnumerator<Wait> IncrementCounter0Ever10Seconds() {
            while (true) {
                yield return new Wait(10);
                counter0++;
            }
        }

        int counter1 = 0;

        IEnumerator<Wait> IncrementCounter1Every5Seconds() {
            while (true) {
                yield return new Wait(5);
                counter1++;
            }
        }

        ActiveCoroutine incCounter0 = CoroutineHandler.Start(IncrementCounter0Ever10Seconds());
        ActiveCoroutine incCounter1 = CoroutineHandler.Start(IncrementCounter1Every5Seconds());
        CoroutineHandler.Tick(3);
        Assert.AreEqual(0, counter0, "Incorrect counter0 value after 3 seconds.");
        Assert.AreEqual(0, counter1, "Incorrect counter1 value after 3 seconds.");
        CoroutineHandler.Tick(3);
        Assert.AreEqual(0, counter0, "Incorrect counter0 value after 6 seconds.");
        Assert.AreEqual(1, counter1, "Incorrect counter1 value after 6 seconds.");

        // it's 5 over here because IncrementCounter1Every5Seconds
        // increments 5 seconds after last yield. not 5 seconds since start.
        // So the when we send 3 seconds in the last SimulateTime,
        // the 3rd second was technically ignored.
        CoroutineHandler.Tick(5);
        Assert.AreEqual(1, counter0, "Incorrect counter0 value after 10 seconds.");
        Assert.AreEqual(2, counter1, "Incorrect counter1 value after next 5 seconds.");

        incCounter0.Cancel();
        incCounter1.Cancel();
    }

    [TestMethod]
    public void InvokeLaterAndNameTest() {
        int counter = 0;
        ActiveCoroutine cr = CoroutineHandler.InvokeLater(new Wait(10), () => {
            counter++;
        }, "Bird");

        CoroutineHandler.Tick(5);
        Assert.AreEqual(0, counter, "Incorrect counter value after 5 seconds.");
        CoroutineHandler.Tick(5);
        Assert.AreEqual(1, counter, "Incorrect counter value after 10 seconds.");
        Assert.IsTrue(cr.IsFinished, "Incorrect IsFinished value.");
        Assert.IsFalse(cr.WasCanceled, "Incorrect IsCanceled value.");
        Assert.AreEqual(2, cr.MoveNextCount, "Incorrect MoveNextCount value.");
        Assert.AreEqual("Bird", cr.Name, "Incorrect name of the coroutine.");
    }

    [TestMethod]
    public void CoroutineStatsAreUpdated() {
        static IEnumerator<Wait> CoroutineTakesMax500Ms() {
            Thread.Sleep(200);
            yield return new Wait(10);
            Thread.Sleep(500);
        }

        ActiveCoroutine cr = CoroutineHandler.Start(CoroutineTakesMax500Ms());
        for (int i = 0; i < 5; i++)
            CoroutineHandler.Tick(50);

        Assert.IsGreaterThanOrEqualTo(700, cr.TotalMoveNextTime.TotalMilliseconds);
        Assert.IsGreaterThanOrEqualTo(500, cr.LastMoveNextTime.TotalMilliseconds);
        Assert.AreEqual(2, cr.MoveNextCount);
    }

    [TestMethod]
    public void TestTickWithNestedAddAndRaiseEvent() {
        var coroutineCreated = new Event();
        int counterCoroutineA = 0;
        int counter = 0;

        ActiveCoroutine infinite = CoroutineHandler.Start(OnCoroutineCreatedInfinite());
        CoroutineHandler.Start(OnEvent1());
        CoroutineHandler.Tick(1);
        CoroutineHandler.Tick(1);
        CoroutineHandler.Tick(1);
        Assert.AreEqual(3, counter);
        Assert.AreEqual(2, counterCoroutineA);
        infinite.Cancel();

        IEnumerator<Wait> OnCoroutineCreatedInfinite() {
            while (true) {
                yield return new Wait(coroutineCreated);
                counterCoroutineA++;
            }
        }

        IEnumerator<Wait> OnEvent1() {
            yield return new Wait(1);
            counter++;
            CoroutineHandler.Start(OnEvent2());
            CoroutineHandler.RaiseEvent(coroutineCreated);
        }

        IEnumerator<Wait> OnEvent2() {
            yield return new Wait(1);
            counter++;
            CoroutineHandler.Start(OnEvent3());
            CoroutineHandler.RaiseEvent(coroutineCreated);
        }

        IEnumerator<Wait> OnEvent3() {
            yield return new Wait(1);
            counter++;
        }
    }

    [TestMethod]
    public void TestTickWithNestedAddAndRaiseEventOnFinish() {
        var onChildCreated = new Event();
        var onParentCreated = new Event();
        int counterAlwaysRunning = 0;

        IEnumerator<Wait> AlwaysRunning() {
            while (true) {
                yield return new Wait(1);
                counterAlwaysRunning++;
            }
        }

        int counterChild = 0;

        IEnumerator<Wait> Child() {
            yield return new Wait(1);
            counterChild++;
        }

        int counterParent = 0;

        IEnumerator<Wait> Parent() {
            yield return new Wait(1);
            counterParent++;
            // OnFinish I will start child.
        }

        int counterGrandParent = 0;

        IEnumerator<Wait> GrandParent() {
            yield return new Wait(1);
            counterGrandParent++;
            // Nested corotuine starting.
            ActiveCoroutine p = CoroutineHandler.Start(Parent());
            CoroutineHandler.RaiseEvent(onParentCreated);
            // Nested corotuine starting in OnFinished.
            p.OnFinished += _ => {
                CoroutineHandler.Start(Child());
                CoroutineHandler.RaiseEvent(onChildCreated);
            };
        }

        ActiveCoroutine always = CoroutineHandler.Start(AlwaysRunning());
        CoroutineHandler.Start(GrandParent());
        Assert.AreEqual(0, counterAlwaysRunning, "Always running counter is invalid at event 0.");
        Assert.AreEqual(0, counterGrandParent, "Grand Parent counter is invalid at event 0.");
        Assert.AreEqual(0, counterParent, "Parent counter is invalid at event 0.");
        Assert.AreEqual(0, counterChild, "Child counter is invalid at event 0.");
        CoroutineHandler.Tick(1);
        Assert.AreEqual(1, counterAlwaysRunning, "Always running counter is invalid at event 1.");
        Assert.AreEqual(1, counterGrandParent, "Grand Parent counter is invalid at event 1.");
        Assert.AreEqual(0, counterParent, "Parent counter is invalid at event 1.");
        Assert.AreEqual(0, counterChild, "Child counter is invalid at event 1.");
        CoroutineHandler.Tick(1);
        Assert.AreEqual(2, counterAlwaysRunning, "Always running counter is invalid at event 2.");
        Assert.AreEqual(1, counterGrandParent, "Grand Parent counter is invalid at event 2.");
        Assert.AreEqual(1, counterParent, "Parent counter is invalid at event 2.");
        Assert.AreEqual(0, counterChild, "Child counter is invalid at event 2.");
        CoroutineHandler.Tick(1);
        Assert.AreEqual(3, counterAlwaysRunning, "Always running counter is invalid at event 3.");
        Assert.AreEqual(1, counterGrandParent, "Grand Parent counter is invalid at event 3.");
        Assert.AreEqual(1, counterParent, "Parent counter is invalid at event 3.");
        Assert.AreEqual(1, counterChild, "Child counter is invalid at event 3.");
        CoroutineHandler.Tick(1);
        Assert.AreEqual(4, counterAlwaysRunning, "Always running counter is invalid at event 4.");
        Assert.AreEqual(1, counterGrandParent, "Grand Parent counter is invalid at event 4.");
        Assert.AreEqual(1, counterParent, "Parent counter is invalid at event 4.");
        Assert.AreEqual(1, counterChild, "Child counter is invalid at event 4.");
        always.Cancel();
    }

}
