using System;
using System.Collections.Generic;

public static class ObservableExtensions 
{ 
    public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext) 
    { 
        return observable.Subscribe(new ActionToObserver<T>(onNext)); 
    } 
}

public class ActionToObserver<T> : IObserver<T> 
{ 
    private readonly Action<T> m_Action; 
    public ActionToObserver(Action<T> action) 
    { 
        m_Action = action; 
    } 
    public void OnCompleted() 
    { } 
    public void OnError(Exception error) 
    { } 
    public void OnNext(T value) 
    { 
        m_Action.Invoke(value); 
    } 

    // Test values for maps:
    // [TestCase("map_1", 0, 2, 2, 18)]
    // [TestCase("map_2", 24, 0, 9, 9, 118)]
}
public class ObservableProperty<T> : IObservable<T> 
{
    private bool m_HasValue = false;
    private T m_Value; 
    private readonly Subject<T> m_Subject = new Subject<T>(); 
    public T Value 
    { 
        get => m_Value; 
        set 
        { 
            if (EqualityComparer<T>.Default.Equals(m_Value, value) == false || m_HasValue == false) 
            {
                m_HasValue = true;
                m_Value = value; 
                m_Subject.OnNext(m_Value); 
            } 
        } 
    } 
    public IDisposable Subscribe(IObserver<T> observer) 
    { 
        return m_Subject.Subscribe(observer); 
    } 
}
public class Subject<T> : ISubject<T>
{
    private int m_Index = 0;
    private readonly List<IObserver<T>> m_Observers = new List<IObserver<T>>(); 
    public void OnCompleted() 
    { 
        for (m_Index = 0; m_Index < m_Observers.Count; m_Index++) 
        {
            m_Observers[m_Index].OnCompleted(); 
        } 
    }

    public void OnError(Exception error) 
    { 
        for (m_Index = 0; m_Index < m_Observers.Count; m_Index++) 
        { 
            m_Observers[m_Index].OnError(error); 
        } 
    }

    public void OnNext(T value) 
    { 
        for (m_Index = 0; m_Index < m_Observers.Count; m_Index++)
        { 
            m_Observers[m_Index].OnNext(value); 
        } 
    }

    public IDisposable Subscribe(IObserver<T> observer) 
    { 
        m_Observers.Add(observer); 
        return new Subscription(this, observer); 
    }
    private class Subscription : IDisposable 
    {
        private readonly Subject<T> m_Subject;
        private readonly IObserver<T> m_Observer; 
        public Subscription(Subject<T> subject, IObserver<T> observer) 
        { 
            m_Subject = subject; 
            m_Observer = observer; 
        } 
        public void Dispose() 
        {
            int elementIndex = m_Subject.m_Observers.IndexOf(m_Observer);
            if(elementIndex < 0)
            {
                m_Subject.m_Observers.Remove(m_Observer);
                if (elementIndex <= m_Subject.m_Index)
                {
                    m_Subject.m_Index++;
                }
            }
        } 
    }
}
public class SubjectCaller
{
    public void SubscribeToSubject()
    { 
        Subject<int> intStream = new Subject<int>();
        IDisposable subscription = intStream.Subscribe(null);
        subscription.Dispose();
    }
}
public interface ISubject<T> : IObservable<T>, IObserver<T> { }