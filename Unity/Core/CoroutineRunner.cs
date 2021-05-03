using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polymorph.Unity.Core {

    public class CoroutineRunner : IDisposable {

        object owner;

        CoroutineHolder runner { get { return CoroutineHolder.holder; } }
        Dictionary<int, Semaphore> semaphores = new Dictionary<int, Semaphore>();
        Dictionary<IEnumerator, CoroutineWrapper> myCoroutines = new Dictionary<IEnumerator, CoroutineWrapper>();

        public CoroutineRunner(object owner = null) {
            if(owner != null) {
                this.owner = owner;
            } else {
                this.owner = this;
            }
        }

        public void Dispose() {
            foreach(var key in myCoroutines.Keys) {
                Halt(myCoroutines[key]);
            }
            myCoroutines.Clear();
            semaphores.Clear();
        }

        public void RunOnNextFrame(Action action) {
            runner.RunOnNextFrame(action);
        }

        public ICoroutine Start(IEnumerator coroutine) {
            var retVal = new CoroutineWrapper(coroutine, owner);
            retVal.OnComplete(CoroutineFinished);
            myCoroutines.Add(coroutine, retVal);
            if(runner != null) {
                retVal.unityObject = runner.StartCoroutine(AdvancedCoroutine(retVal));
            }
            return retVal;
        }

        public void Stop(ICoroutine coroutine) {
            if(coroutine != null) {
                var wrapper = coroutine as CoroutineWrapper;
                if((wrapper != null) && (wrapper.routine != null)) {
                    if(myCoroutines.ContainsKey(wrapper.routine)) {
                        if(myCoroutines.Remove(wrapper.routine)) {
                            Halt(wrapper);
                        }
                    }
                }
            }
        }

        public void Stop(IEnumerator coroutine) {
            Stop(myCoroutines[coroutine]);
        }

        void Halt(CoroutineWrapper wrapper) {
            if(runner != null) {
                runner.StopCoroutine(wrapper.unityObject);
            }
            wrapper.Dispose();
        }

        void CoroutineFinished(ICoroutine coroutine) {
            var wrapper = coroutine as CoroutineWrapper;
            if(myCoroutines.Remove(wrapper.routine)) {
                wrapper.Dispose();
            }
        } 

        IEnumerator AdvancedCoroutine(CoroutineWrapper coObj) {

            var keepGoing = true;
            while(keepGoing) {

                if(coObj.owner.Equals(null)) { // Make sure owner didnt die
                    coObj.Killed();
                    yield break;
                }

                if(coObj.ShouldStop()) { // Check if kill command issued
                    yield break;
                }

                #region Do next step
                try { // Do next step, self destruct on error
                    keepGoing = coObj.routine.MoveNext();
                    if(!keepGoing) {
                        break;
                    }
                } catch(Exception e) {
                    Debug.LogException(e);
                    coObj.Killed();
                    yield break;
                }
                #endregion
                #region Aquire Semaphore
                if(coObj.routine.Current is BaseAquire) { // Semaphore was requested by the coroutine

                    var semaphoreRequest = coObj.routine.Current as BaseAquire;
                    int hash;

                    if(semaphoreRequest.hash == null) {
                        hash = coObj.GetHashCode();
                    } else {
                        hash = semaphoreRequest.hash.GetHashCode();
                    }

                    // Find or create a semaphore for the coroutine
                    if(semaphores.ContainsKey(hash)) {
                        coObj.semaphore = semaphores[hash];
                    } else {
                        coObj.semaphore = new Semaphore();
                        semaphores.Add(hash, coObj.semaphore);
                    }
                    ++coObj.semaphore.index;
                    coObj.semIndex = coObj.semaphore.index;
                    // Debug.Log(coObj.semIndex);
                    semaphoreRequest.SetSemaphore(coObj.semaphore);

                    while(true) {
                        var aquireState = semaphoreRequest.AttemptAquire(); // Hand over control to BaseAquire subclass until aquired or killed
                        if(aquireState == AquireState.Aquired) {
                            break;
                        } else if(aquireState == AquireState.Kill) {
                            coObj.Killed();
                            yield break;
                        }
                        yield return null;
                    }

                    coObj.semaphore.running = true; // Set me as the running coroutine
                    coObj.semaphore.kill = false; // Reset kill flag
                    continue;
                }
                #endregion
                #region Yield to another coroutine
                if(coObj.routine.Current is ICoroutine) { // Yielded to another coroutine, wait for it to finish
                    var otherRoutine = coObj.routine.Current as ICoroutine;
                    while(!otherRoutine.complete) {
                        if(coObj.ShouldStop()) {
                            yield break;
                        }
                        yield return null;
                    }
                }
                #endregion

                // Need to include my own handles of unity yield instructions as semapphores will stop functioning when they are used
                yield return coObj.routine.Current; // Hand over control to unity for specific yield instructions
            }
            coObj.Finished();
        }
    }
}
