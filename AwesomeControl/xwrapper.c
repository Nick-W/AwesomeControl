//Simple wrapper for XCreateWindow by Nick Wilson
//Compile with 'gcc -fPIC -shared xwrapper.c -o xwrapper.so'
//Use with 'LD_PRELOAD=xwrapper.so command'

#define _GNU_SOURCE

#include <X11/X.h>
#include <X11/Xlib.h>
#include <X11/Xutil.h>
#include <dlfcn.h>
#include <stdio.h>
#include <stdlib.h>


Window XCreateWindow(Display *display,
                     Window parent,
                     int x,
                     int y,
                     unsigned int width,
                     unsigned int height,
                     unsigned int border_width,
                     int depth,
                     unsigned int class,
                     Visual *visual,
                     unsigned long valuemask,
                     XSetWindowAttributes* attributes)
{
        static Window (*XCreateWindow_real)(Display *display,
                     Window parent,
                     int x,
                     int y,
                     unsigned int width,
                     unsigned int height,
                     unsigned int border_width,
                     int depth,
                     unsigned int class,
                     Visual *visual,
                     unsigned long valuemask,
                     XSetWindowAttributes* attributes)=NULL;
        if (!XCreateWindow_real) XCreateWindow_real=dlsym(RTLD_NEXT,"XCreateWindow");
        Window w = XCreateWindow_real(display,parent,x,y,width,height,border_width,depth,class,visual,valuemask,attributes);
        fprintf(stderr,"HWND=%#010x\n",(uint)w);
        return w;
}
