using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using System.Threading.Tasks;
using Android.Graphics.Drawables;
using System.IO;

namespace RoundedCornerImage
{
	[Activity ( Label = "RoundedCornerImage" , MainLauncher = true , Icon = "@drawable/icon" )]
	public class MainActivity : Activity
	{

		string strImgPath;
		async protected override void OnCreate ( Bundle bundle )
		{
			base.OnCreate ( bundle ); 

			// Set our view from the "main" layout resource
			SetContentView ( Resource.Layout.Main );
			ImageView imgProfilePic = FindViewById<ImageView> ( Resource.Id.myImg );
		 
			imgProfilePic.Click +=async delegate(object sender , EventArgs e )
			{
				Console.WriteLine(imgProfilePic.Width + " MeasuredWidth "+imgProfilePic.MeasuredWidth);
				Console.WriteLine(imgProfilePic.Height + " MeasuredHeight "+imgProfilePic.MeasuredHeight);
					
				strImgPath =System.IO.Path.Combine( Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,"Sample","aa") ;

//				if (! System.IO.Directory.Exists ( strImgPath ) )
//					System.IO.Directory.CreateDirectory ( strImgPath );

				strImgPath =System.IO.Path.Combine ( strImgPath , "sample.jpg" ); 

				int width=imgProfilePic.MeasuredWidth/2;//imgProfilePic.Width;
				int height=imgProfilePic.MeasuredHeight/2;
				BitmapFactory.Options option = await  GetBitmapOptionsOfImage ( strImgPath );
			 
				var bitmapSampled = await LoadScaledDownBitmapForDisplayAsync ( strImgPath , option , width , height ); 
				Console.WriteLine("bitmap width "+bitmapSampled.Width+" height : "+bitmapSampled.Height);
				//to get cirlcle shape H*W should be same 
				int intRoundPicResolution; 
				if(bitmapSampled.Height>bitmapSampled.Width)  //only to get circle shape
					intRoundPicResolution=bitmapSampled.Width;
				else
					intRoundPicResolution=bitmapSampled.Height;
				
				bitmapSampled = Bitmap.CreateScaledBitmap ( bitmapSampled , intRoundPicResolution , intRoundPicResolution , false );
				int pixel2 = ( bitmapSampled.Width ) / 2; 
				using ( var bitmapRoundedCorner = GetRoundedCornerBitmap ( bitmapSampled , pixel2 ) )
				{
					RecycleBitmap (imgProfilePic);
					imgProfilePic.SetImageBitmap ( bitmapRoundedCorner ); 
				}  
				try{
					
				 string strNewImgPath =System.IO.Path.Combine ( Android.OS.Environment.ExternalStorageDirectory.AbsolutePath ,"Sample", "newsample.jpg" ); 
//					File.Copy(strImgPath,strNewImgPath,true);
//					strNewImgPath= Directory.GetParent(strImgPath).FullName;
					FileStream fileStream = new FileStream (strNewImgPath, FileMode.Create);
					bitmapSampled.Compress (Bitmap.CompressFormat.Png, 80, fileStream);//writing file in local with scaled down bitmap
				    fileStream.Close ();

				bitmapSampled.Dispose();
				bitmapSampled=null; 

				}
				catch(Exception es)
				{ 
					Console.WriteLine(es.Message);
				}
			};
		}

		// If you would like to create a circle of the image set pixels to half the width of the image.
		internal static   Bitmap GetRoundedCornerBitmap(Bitmap bitmap, int pixels)
		{
			Bitmap output = null;

			try
			{
				output = Bitmap.CreateBitmap(bitmap.Width, bitmap.Height, Bitmap.Config.Argb8888);
				Canvas canvas = new Canvas(output);

				Color color = new Color(66, 66, 66);
				Paint paint = new Paint();
				Rect rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
				RectF rectF = new RectF(rect);
				float roundPx = pixels;

				paint.AntiAlias = true;
				canvas.DrawARGB(0, 0, 0, 0);
				paint.Color = color;
				canvas.DrawRoundRect(rectF, roundPx, roundPx, paint);

				paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
				canvas.DrawBitmap(bitmap, rect, rect, paint);
			}
			catch (System.Exception err)
			{
				System.Console.WriteLine ("GetRoundedCornerBitmap Error - " + err.Message);
			}

			return output;
		}
		//to get bitmap option current image with it's height and width 
		internal async static Task<BitmapFactory.Options> GetBitmapOptionsOfImage(string strFileName)
		{
			BitmapFactory.Options options = new BitmapFactory.Options
			{
				InJustDecodeBounds = true //avoids memory allocation during decoding
			}; 
			// The result will be null because InJustDecodeBounds == true.
			await BitmapFactory.DecodeFileAsync(strFileName, options);

			int imageHeight = options.OutHeight;
			int imageWidth = options.OutWidth;
			Console.WriteLine ( "height : " + imageHeight + " width : " + imageWidth );
			//			_originalDimensions.Text = string.Format("Original Size= {0}x{1}", imageWidth, imageHeight); 
			return options;
		}
		internal async static Task<Bitmap> LoadScaledDownBitmapForDisplayAsync(string strFileName, BitmapFactory.Options options, int reqWidth, int reqHeight)
		{
			// Calculate inSampleSize
			options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);
			// Decode bitmap with inSampleSize set
			options.InJustDecodeBounds = false; //to let memory allocation during decoding 
			return await BitmapFactory.DecodeFileAsync(strFileName,options);
		}
		//Calculates InSampleSize ratio if required
		internal static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
		{
			// Raw height and width of image
			float height = options.OutHeight;
			float width = options.OutWidth;
			double inSampleSize = 1D;

			if (height > reqHeight || width > reqWidth)
			{
				int halfHeight = (int)(height / 2);
				int halfWidth = (int)(width / 2);

				// Calculate a inSampleSize that is a power of 2 - the decoder will use a value that is a power of two anyway.
				while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
				{
					inSampleSize *= 2;
				} 
			} 
			Console.WriteLine ( "In sample size : "+inSampleSize );
			return (int)inSampleSize;
		} 
		/// <summary>
		/// This method will recyle the memory help by a bitmap in an ImageView
		/// </summary>
		/// <param name="imageView">Image view.</param>
		internal static void RecycleBitmap( ImageView imageView)
		{
			if (imageView == null) {
				return;
			}

			Drawable toRecycle = imageView.Drawable;
			if (toRecycle != null) {
				((BitmapDrawable)toRecycle).Bitmap.Recycle ();
			}
		}
	}
}


