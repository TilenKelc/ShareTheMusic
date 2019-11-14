package md5c539bdc79f76d0c80e6cd44011eba829;


public class StreamMediaDataSource
	extends android.media.MediaDataSource
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_getSize:()J:GetGetSizeHandler\n" +
			"n_readAt:(J[BII)I:GetReadAt_JarrayBIIHandler\n" +
			"n_close:()V:GetCloseHandler\n" +
			"";
		mono.android.Runtime.register ("ShareTheMusic.Droid.StreamMediaDataSource, ShareTheMusic.Android", StreamMediaDataSource.class, __md_methods);
	}


	public StreamMediaDataSource ()
	{
		super ();
		if (getClass () == StreamMediaDataSource.class)
			mono.android.TypeManager.Activate ("ShareTheMusic.Droid.StreamMediaDataSource, ShareTheMusic.Android", "", this, new java.lang.Object[] {  });
	}


	public long getSize ()
	{
		return n_getSize ();
	}

	private native long n_getSize ();


	public int readAt (long p0, byte[] p1, int p2, int p3)
	{
		return n_readAt (p0, p1, p2, p3);
	}

	private native int n_readAt (long p0, byte[] p1, int p2, int p3);


	public void close ()
	{
		n_close ();
	}

	private native void n_close ();

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
