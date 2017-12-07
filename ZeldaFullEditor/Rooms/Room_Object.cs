﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ZeldaFullEditor
{
    [Serializable]
    public class Room_Object
    {
        public byte x, y; //position of the object in the room (*8 for draw)
        public byte nx, ny;
        public byte ox, oy;
        public byte size; //size of the object
        public bool allBgs = false; //if the object is drawn on BG1 and BG2 regardless of type of BG
        public List<Tile> tiles = new List<Tile>();
        public short id; 
        public string name; //name of the object will be shown on the form
        public byte layer = 0;
        public Room room;
        public int drawYFix = 0;
        public bool is_stair = false;
        public bool is_chest = false;
        public bool is_bgr = false;
        public bool redraw = false;
        public bool is_door = false;
        public bool checksize = false;
        public bool selected = false;
        public Bitmap bitmap;
        int lowerX = 0;
        int lowerY = 0;
        int higherX = 0;
        int higherY = 0;
        public int width = 16;
        public int height = 16;
        public byte scroll_x = 2;
        public byte scroll_y = 2;
        public byte base_width = 2;
        public byte base_height = 2;
        public Room_Object(short id,byte x,byte y,byte size,byte layer = 0)
        {
            this.x = x;
            this.y = y;
            this.size = size;
            this.id = id;
            this.layer = layer;
            this.nx = x;
            this.ny = y;
            this.ox = x;
            this.oy = y;


        }

        public void get_scroll_x()
        {
            byte oldSize = size;
            size = 1;
            checksize = true;
            Draw();
            scroll_x = (byte)((width / 8) / 2);
            size = 0;
            resetSize();
            Draw();
            base_width = (byte)(width / 8);
            size = oldSize;
            resetSize();
            checksize = false;
        }


        public void get_scroll_y()
        {
            byte oldSize = size;
            size = 1;
            checksize = true;
            Draw();
            scroll_y = (byte)((height / 8) / 2);
            size = 0;
            resetSize();
            Draw();
            base_height = (byte)(height / 8);
            size = oldSize;
            resetSize();
            checksize = false;
        }

        public void setRoom(Room r)
        {
            this.room = r;
        }

        public virtual void Draw()
        {
             
        }

        /*public void DrawOnBitmap()
        {
            checksize = false;
            Draw(); //check the size of the image
            checksize = true;
            bitmap = new Bitmap(width, height,PixelFormat.Format32bppArgb);
            GFX.begin_draw(bitmap, width, height);
            Draw();
            GFX.end_draw(bitmap);
        }*/

        public void resetSize()
        {
            width = 16;
            height = 16;
        }

        public void addTiles(int nbr,int pos)
        {
            for(int i = 0;i<nbr;i++)
            {
                tiles.Add(new Tile(ROM.DATA[pos + ((i * 2))], ROM.DATA[pos + ((i * 2)) +1]));
            }
        }

        public void draw_diagonal_up()
        {
            for (int s = 0; s < size + 6; s++)
            {
                draw_tile(tiles[0], ( (s)) * 8, (0 - s) * 8,((size+6)*8));
                draw_tile(tiles[1], ( (s)) * 8, (1 - s) * 8, ((size + 6) * 8));
                draw_tile(tiles[2], ( (s)) * 8, (2 - s) * 8, ((size + 6) * 8));
                draw_tile(tiles[3], ( (s)) * 8, (3 - s) * 8, ((size + 6) * 8));
                draw_tile(tiles[4], ( (s)) * 8, (4 - s) * 8, ((size + 6) * 8));
                drawYFix = -(size+6);
            }
        }

        public void draw_diagonal_down()
        {
            for (int s = 0; s < size + 6; s++)
            {
                draw_tile(tiles[0], ( (s)) * 8, (0 + s) * 8);
                draw_tile(tiles[1], ( (s)) * 8, (1 + s) * 8);
                draw_tile(tiles[2], ( (s)) * 8, (2 + s) * 8);
                draw_tile(tiles[3], ( (s)) * 8, (3 + s) * 8);
                draw_tile(tiles[4], ( (s)) * 8, (4 + s) * 8);
            }
        }
        //Object Initialization (Tiles and special stuff)
        public void init_objects()
        {

        }

        public void updatePos()
        {
            this.x = nx;
            this.y = ny;
        }


        public void draw_tile(Tile t, int x, int y, int yfix = 0)
        {

            int zx = x + 8;
            int zy = y + 8;

            if (zx > width)
            {
                width = zx;
            }
            if (zy > height)
            {
                height = zy;
            }
            if (checksize)
            {
                return;
            }

                int ty = (t.id / 16);
                int tx = t.id - (ty * 16);
                int mx = 0;
                int my = 0;


                if (t.mirror_x == true)
                {
                    mx = 8;
                }

                for (int xx = 0; xx < 8; xx++)
                {
                    if (mx > 0)
                    {
                        mx--;
                    }
                    if (t.mirror_y == true)
                    {
                        my = 8;
                    }
                    for (int yy = 0; yy < 8; yy++)
                    {
                        if (my > 0)
                        {
                            my--;
                        }
                        //int x_dest = ((this.x * 8) + x + (xx)) * 4;
                        //int y_dest = (((this.y * 8) + y + (yy)) * 512) * 4;

                        int x_dest = ((this.nx * 8) + x + (xx)) * 4;
                        int y_dest = (((this.ny * 8)+(y) + (yy)) * 512) * 4;
                        int dest = x_dest + y_dest;

                        int x_src = ((tx * 8) + mx + (xx));
                        if (t.mirror_x)
                        {
                            x_src = ((tx * 8) + mx);
                        }
                        int y_src = (((ty * 8) + my + (yy)) * 128);
                        if (t.mirror_y)
                        {
                            y_src = (((ty * 8) + my) * 128);
                        }

                        int src = x_src + y_src;
                        int pp = 0;
                        if (src < 16384)
                        {
                            pp = 8;
                        }
                    if (dest < (1048576))
                        {
                            byte alpha = 255;

                            if (GFX.singledata[(src)] == 0)
                            {
                                if (room.bg2 != 0)
                                {
                                    if (layer != 1)
                                    {
                                        alpha = 0;
                                    }
                                }
                                if (room.bg2 == Background2.OnTop)
                                {
                                    if (layer != 0)
                                    {
                                        alpha = 0;
                                    }
                                    else
                                    {
                                        alpha = 255;
                                    }
                                }
                                if (room.bg2 == Background2.Transparent)
                                {
                                    alpha = 0;
                                }

                            }
                            else
                            {
                                if (room.bg2 == Background2.Transparent)
                                {
                                    if (layer == 1)
                                    {
                                        alpha = 128;
                                    }
                                }
                            }

                            if (allBgs)
                            {
                                alpha = 255;
                            }
                        unsafe
                        {
                            GFX.currentData[dest] = (GFX.loadedPalettes[GFX.singledata[(src)] + pp, t.palette].B);
                            GFX.currentData[dest + 1] = (GFX.loadedPalettes[GFX.singledata[(src)] + pp, t.palette].G);
                            GFX.currentData[dest + 2] = (GFX.loadedPalettes[GFX.singledata[(src)] + pp, t.palette].R);
                            GFX.currentData[dest + 3] = alpha;//A
                        }
                        }
                    }
                }
        }



    }



}
