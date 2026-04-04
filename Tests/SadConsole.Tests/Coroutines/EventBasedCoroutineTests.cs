using System.Collections.Generic;
using SadConsole.Coroutine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SadConsole.Tests.Coroutines;

[TestClass]
public class EventBasedCoroutineTests {

    [TestMethod]
    public void TestEventBasedCoroutine() {
        int counter = 0;
        Event myEvent = new();

        IEnumerator<Wait> OnEventTriggered() {
            counter++;
            yield return new Wait(myEvent);
            counter++;
        }

        ActiveCoroutine cr = CoroutineHandler.Start(OnEventTriggered());
        Assert.AreEqual(1, counter, "instruction before yield is not executed.");
        CoroutineHandler.Tick(1);
        CoroutineHandler.RaiseEvent(myEvent);
        Assert.AreEqual(2, counter, "instruction after yield is not executed.");
        CoroutineHandler.Tick(1);
        CoroutineHandler.RaiseEvent(myEvent);
        Assert.AreEqual(2, counter, "instruction after yield is not executed.");

        Assert.IsTrue(cr.IsFinished, "Incorrect IsFinished value.");
        Assert.IsFalse(cr.WasCanceled, "Incorrect IsCanceled value.");
        Assert.AreEqual(2, cr.MoveNextCount, "Incorrect MoveNextCount value.");
    }

    [TestMethod]
    public void TestInfiniteCoroutineNeverFinishesUnlessCanceled() {
        Event myEvent = new();
        Event myOtherEvent = new();
        int counter = 0;

        IEnumerator<Wait> OnEventTriggeredInfinite() {
            while (true) {
                counter++;
                yield return new Wait(myEvent);
            }
        }

        void SetCounterToUnreachableValue(ActiveCoroutine coroutine) {
            counter = -100;
        }

        ActiveCoroutine cr = CoroutineHandler.Start(OnEventTriggeredInfinite());
        CoroutineHandler.Tick(1);
        cr.OnFinished += SetCounterToUnreachableValue;
        for (int i = 0; i < 50; i++)
            CoroutineHandler.RaiseEvent(myOtherEvent);

        for (int i = 0; i < 50; i++)
            CoroutineHandler.RaiseEvent(myEvent);

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
        Event myEvent = new();
        int counter = 0;

        IEnumerator<Wait> OnEvent() {
            counter++;
            yield return new Wait(myEvent);
        }

        void SetCounterToUnreachableValue(ActiveCoroutine coroutine) {
            counter = -100;
        }

        ActiveCoroutine cr = CoroutineHandler.Start(OnEvent());
        CoroutineHandler.Tick(1);
        cr.OnFinished += SetCounterToUnreachableValue;
        for (int i = 0; i < 10; i++)
            CoroutineHandler.RaiseEvent(myEvent);
        Assert.AreEqual(-100, counter, "Incorrect counter value.");
    }

    [TestMethod]
    public void TestNestedCoroutine() {
        Event onChildCreated = new();
        Event onParentCreated = new();
        Event myEvent = new();
        int counterAlwaysRunning = 0;

        IEnumerator<Wait> AlwaysRunning() {
            while (true) {
                yield return new Wait(myEvent);
                counterAlwaysRunning++;
            }
        }

        int counterChild = 0;

        IEnumerator<Wait> Child() {
            yield return new Wait(myEvent);
            counterChild++;
        }

        int counterParent = 0;

        IEnumerator<Wait> Parent() {
            yield return new Wait(myEvent);
            counterParent++;
            // OnFinish I will start child.
        }

        int counterGrandParent = 0;

        IEnumerator<Wait> GrandParent() {
            yield return new Wait(myEvent);
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
        CoroutineHandler.RaiseEvent(myEvent);
        Assert.AreEqual(1, counterAlwaysRunning, "Always running counter is invalid at event 1.");
        Assert.AreEqual(1, counterGrandParent, "Grand Parent counter is invalid at event 1.");
        Assert.AreEqual(0, counterParent, "Parent counter is invalid at event 1.");
        Assert.AreEqual(0, counterChild, "Child counter is invalid at event 1.");
        CoroutineHandler.Tick(1);
        CoroutineHandler.RaiseEvent(myEvent);
        Assert.AreEqual(2, counterAlwaysRunning, "Always running counter is invalid at event 2.");
        Assert.AreEqual(1, counterGrandParent, "Grand Parent counter is invalid at event 2.");
        Assert.AreEqual(1, counterParent, "Parent counter is invalid at event 2.");
        Assert.AreEqual(0, counterChild, "Child counter is invalid at event 2.");
        CoroutineHandler.Tick(1);
        CoroutineHandler.RaiseEvent(myEvent);
        Assert.AreEqual(3, counterAlwaysRunning, "Always running counter is invalid at event 3.");
        Assert.AreEqual(1, counterGrandParent, "Grand Parent counter is invalid at event 3.");
        Assert.AreEqual(1, counterParent, "Parent counter is invalid at event 3.");
        Assert.AreEqual(1, counterChild, "Child counter is invalid at event 3.");
        CoroutineHandler.Tick(1);
        CoroutineHandler.RaiseEvent(myEvent);
        Assert.AreEqual(4, counterAlwaysRunning, "Always running counter is invalid at event 4.");
        Assert.AreEqual(1, counterGrandParent, "Grand Parent counter is invalid at event 4.");
        Assert.AreEqual(1, counterParent, "Parent counter is invalid at event 4.");
        Assert.AreEqual(1, counterChild, "Child counter is invalid at event 4.");
        always.Cancel();
    }

    [TestMethod]
    public void TestNestedRaiseEvent() {
        Event event1 = new();
        Event event2 = new();
        Event event3 = new();
        Event coroutineCreated = new();
        int counterCoroutineA = 0;
        int counter = 0;

        ActiveCoroutine infinite = CoroutineHandler.Start(OnCoroutineCreatedInfinite());
        CoroutineHandler.Start(OnEvent1());
        CoroutineHandler.Tick(1);
        CoroutineHandler.RaiseEvent(event1);
        CoroutineHandler.Tick(1);
        CoroutineHandler.RaiseEvent(event2);
        CoroutineHandler.Tick(1);
        CoroutineHandler.RaiseEvent(event3);
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
            yield return new Wait(event1);
            counter++;
            CoroutineHandler.Start(OnEvent2());
            CoroutineHandler.RaiseEvent(coroutineCreated);
        }

        IEnumerator<Wait> OnEvent2() {
            yield return new Wait(event2);
            counter++;
            CoroutineHandler.Start(OnEvent3());
            CoroutineHandler.RaiseEvent(coroutineCreated);
        }

        IEnumerator<Wait> OnEvent3() {
            yield return new Wait(event3);
            counter++;
        }
    }

    [TestMethod]
    public void TestPriority() {
        Event myEvent = new();
        int counterShouldExecuteBefore0 = 0;

        IEnumerator<Wait> ShouldExecuteBefore0() {
            while (true) {
                yield return new Wait(myEvent);
                counterShouldExecuteBefore0++;
            }
        }

        int counterShouldExecuteBefore1 = 0;

        IEnumerator<Wait> ShouldExecuteBefore1() {
            while (true) {
                yield return new Wait(myEvent);
                counterShouldExecuteBefore1++;
            }
        }

        int counterShouldExecuteAfter = 0;

        IEnumerator<Wait> ShouldExecuteAfter() {
            while (true) {
                yield return new Wait(myEvent);
                if (counterShouldExecuteBefore0 == 1 &&
                    counterShouldExecuteBefore1 == 1) {
                    counterShouldExecuteAfter++;
                }
            }
        }

        int counterShouldExecuteFinally = 0;

        IEnumerator<Wait> ShouldExecuteFinally() {
            while (true) {
                yield return new Wait(myEvent);
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
        CoroutineHandler.Tick(1);
        CoroutineHandler.RaiseEvent(myEvent);
        Assert.AreEqual(1, counterShouldExecuteAfter, $"ShouldExecuteAfter counter  {counterShouldExecuteAfter} is invalid.");
        Assert.AreEqual(1, counterShouldExecuteFinally, $"ShouldExecuteFinally counter  {counterShouldExecuteFinally} is invalid.");

        before1.Cancel();
        after.Cancel();
        before0.Cancel();
        @finally.Cancel();
    }

    [TestMethod]
    public void InvokeLaterAndNameTest() {
        Event myEvent = new();
        int counter = 0;
        ActiveCoroutine cr = CoroutineHandler.InvokeLater(new Wait(myEvent), () =>
        {
            counter++;
        }, "Bird");

        CoroutineHandler.InvokeLater(new Wait(myEvent), () => {
            counter++;
        });

        CoroutineHandler.InvokeLater(new Wait(myEvent), () => {
            counter++;
        });

        Assert.AreEqual(0, counter, "Incorrect counter value after 5 seconds.");
        CoroutineHandler.Tick(1);
        CoroutineHandler.RaiseEvent(myEvent);
        Assert.AreEqual(3, counter, "Incorrect counter value after 10 seconds.");
        Assert.IsTrue(cr.IsFinished, "Incorrect IsFinished value.");
        Assert.IsFalse(cr.WasCanceled, "Incorrect IsCanceled value.");
        Assert.AreEqual(2, cr.MoveNextCount, "Incorrect MoveNextCount value.");
        Assert.AreEqual("Bird", cr.Name, "Incorrect name of the coroutine.");
    }

    [TestMethod]
    public void MovingCoroutineTest() {
        Event evt = new();

        IEnumerator<Wait> MovingCoroutine() {
            while (true) {
                yield return new Wait(evt);
                yield return new Wait(0d);
            }
        }

        ActiveCoroutine moving = CoroutineHandler.Start(MovingCoroutine(), "MovingCoroutine");
        CoroutineHandler.RaiseEvent(evt);
        CoroutineHandler.RaiseEvent(evt);
        CoroutineHandler.RaiseEvent(evt);
        CoroutineHandler.RaiseEvent(evt);

        CoroutineHandler.Tick(1d);
        CoroutineHandler.Tick(1d);
        CoroutineHandler.Tick(1d);
        CoroutineHandler.Tick(1d);

        CoroutineHandler.RaiseEvent(evt);
        CoroutineHandler.Tick(1d);
        CoroutineHandler.RaiseEvent(evt);
        CoroutineHandler.Tick(1d);
        CoroutineHandler.RaiseEvent(evt);
        CoroutineHandler.Tick(1d);

        CoroutineHandler.Tick(1d);
        CoroutineHandler.RaiseEvent(evt);
        CoroutineHandler.Tick(1d);
        CoroutineHandler.RaiseEvent(evt);
        CoroutineHandler.Tick(1d);
        CoroutineHandler.RaiseEvent(evt);

        moving.Cancel();
    }

}
