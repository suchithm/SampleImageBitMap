using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Java.Lang;
using System.Threading.Tasks;
using Android.Graphics.Drawables;


namespace RoundedCornerImage
{
	internal static class ImageHelper
	{
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
			return await BitmapFactory.DecodeFileAsync(strFileName, options);
		}
		//Calculates InSampleSize if required
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
			return (int)inSampleSize;
		} 
		/// <summary>
		/// This method will recyle the memory help by a bitmap in an ImageView
		/// </summary>
		/// <param name="imageView">Image view.</param>
		/// _imageView.RecycleBitmap ();
		internal static void RecycleBitmap(this ImageView imageView)
		{
			if (imageView == null) {
				return;
			} 
			Drawable toRecycle = imageView.Drawable;
			if (toRecycle != null) {
				((BitmapDrawable)toRecycle).Bitmap.Recycle ();
			}
		}

	//---- Non async operation---------

		//to get bitmap option current image with it's height and width 
		internal  static BitmapFactory.Options GetBitmapOptionsOfImageNonAsync(string strFileName)
		{
			BitmapFactory.Options options = new BitmapFactory.Options
			{
				InJustDecodeBounds = true //avoids memory allocation during decoding
			}; 
			// The result will be null because InJustDecodeBounds == true.
			BitmapFactory.DecodeFile(strFileName, options); 
			int imageHeight = options.OutHeight;
			int imageWidth = options.OutWidth;
			Console.WriteLine ( "height : " + imageHeight + " width : " + imageWidth ); 
			return options;
		}
		internal  static Bitmap LoadScaledDownBitmapForDisplay(string strFileName, BitmapFactory.Options options, int reqWidth, int reqHeight)
		{
			// Calculate inSampleSize
			options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);
			// Decode bitmap with inSampleSize set
			options.InJustDecodeBounds = false; //to let memory allocation during decoding 
			return  BitmapFactory.DecodeFile(strFileName,options);
		}

 
	}
}

