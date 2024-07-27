package com.capstoneproject2024.kok;

import android.content.Context;
import android.os.Environment;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;

public class SaveToMusicFolder {
    public static boolean saveFileToMusicFolder(Context context, InputStream inputStream, String fileName) {
        if (Environment.getExternalStorageState().equals(Environment.MEDIA_MOUNTED)) {
            File musicDir = Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_MUSIC);
            File kokDir = new File(musicDir, "KOK/Recording");
            if (!kokDir.exists()) {
                if (!kokDir.mkdirs()) {
                    return false;
                }
            }

            File file = new File(kokDir, fileName);
            try (FileOutputStream fos = new FileOutputStream(file)) {
                byte[] buffer = new byte[1024];
                int length;
                while ((length = inputStream.read(buffer)) > 0) {
                    fos.write(buffer, 0, length);
                }
                return true;
            } catch (IOException e) {
                e.printStackTrace();
                return false;
            }
        }
        return false;
    }
}
