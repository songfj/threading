#include <stdio.h>
#include <math.h>
#include <windows.h>
#include <gl\glut.h>
#include <gl\gl.h>
#include "main.h"
#include "E:\\others\\xampp\\mysql\\include\\mysql.h"
#pragma comment(lib,"..\\Debug\\libmysql.lib")

#define CALCULATEAMOUNT1 4193
#define CALCULATEAMOUNT2 4192

float x, y;
int count1 = 0, count2 = 0;

/*将计算的点存入数据*/
void CalculatePoint1(float sourceX, float sourceY, float destinationX, float destinationY, MYSQL mysql)
{
    float currentx = sourceX, currenty=sourceY;
    float lastx, lasty;
    float step = 0.1;
    float k, b;
    char sql[200];/*存储sql语句*/

    if(sourceX==destinationX)
    {
        lastx = sourceX;
        while(true)
        {
            currenty -= 0.1;
            if(currenty-destinationY<0.001) break;
         
            lasty = currenty;
            printf("%f, %f\n", currentx, currenty);
            sprintf(sql, "INSERT INTO point1(f_Point_X, f_Point_Y) values(%f, %f)", currentx, currenty);
            mysql_query(&mysql, sql);
            count1++;
        }
    }
    else
    {
        k=(sourceY-destinationY)/(sourceX-destinationX);
        b=destinationY-k*destinationX;

        if(sourceX < destinationX)
        {
            step = 0.1;
        }
        else
        {
            step = -0.1;
        }

        while(true)
        {
            /*画线*/
            lastx = currentx;
            lasty = currenty;

            printf("%f, %f\n", currentx, currenty);

            sprintf(sql, "INSERT INTO point1(f_Point_X, f_Point_Y) values(%f, %f)", currentx, currenty);
            mysql_query(&mysql, sql);
            count1++;

            /*就算下一个点*/
            currentx += step;
            currenty=k*currentx+b;

            if(abs(step-(-0.1)) < 0.01)
            {
                if(currentx-destinationX<0.001) break;
            }
            else
            {
                if(destinationX-currentx<0.001) break;
            }
        }
    }//end else
}

/*将计算的点存入数据*/
void CalculatePoint2(float sourceX, float sourceY, float destinationX, float destinationY, MYSQL mysql)
{
    float currentx = sourceX, currenty=sourceY;
    float lastx, lasty;
    float step = 0.1;
    float k, b;
    char sql[200];/*存储sql语句*/

    if(sourceX==destinationX)
    {
        lastx = sourceX;
        while(true)
        {
            currenty -= 0.1;
            if(currenty-destinationY<0.001) break;
            printf("%f, %f\n", currentx, currenty);
            lasty = currenty;
            sprintf(sql, "INSERT INTO point2(f_Point_X, f_Point_Y) values(%f, %f)", currentx, currenty);
            mysql_query(&mysql, sql);
            count2++;
        }
    }
    else
    {
        k=(sourceY-destinationY)/(sourceX-destinationX);
        b=destinationY-k*destinationX;

        if(sourceX < destinationX)
        {
            step = 0.1;
        }
        else
        {
            step = -0.1;
        }
        while(true)
        {
            /*画线*/
            lastx = currentx;
            lasty = currenty;

            sprintf(sql, "INSERT INTO point2(f_Point_X, f_Point_Y) values(%f, %f)", currentx, currenty);
            mysql_query(&mysql, sql);
            count2++;

            /*就算下一个点*/
            currentx += step;
            currenty=k*currentx+b;

            if(abs(step-(-0.1)) < 0.01)
            {
                if(currentx-destinationX<0.001) break;
            }
            else
            {
                if(destinationX-currentx<0.001) break;
            }
        }
    }//end else
}

/*获得左边的点*/
void GetPoint1(MYSQL mysql)
{
    /*清空数据库中的数据*/
    mysql_query(&mysql, "TRUNCATE point1");
    count1 = 0;

    CalculatePoint1(60, 100, 40, 80, mysql);
    CalculatePoint1(40, 80, 50, 80, mysql);
    CalculatePoint1(50, 80, 20, 50, mysql);
    CalculatePoint1(20, 50, 50, 50, mysql);
    CalculatePoint1(50, 50, 0, 20, mysql);
    CalculatePoint1(0, 20, 50, 20, mysql);
    CalculatePoint1(50, 20, 50, 0, mysql);
}

/*获得右边的点*/
void GetPoint2(MYSQL mysql)
{
    /*清空数据库中的数据*/
    mysql_query(&mysql, "TRUNCATE point2");
    count2 = 0;

    CalculatePoint2(60, 100, 80, 80, mysql);
    CalculatePoint2(80, 80, 70, 80, mysql);
    CalculatePoint2(70, 80, 100, 50, mysql);
    CalculatePoint2(100, 50, 70, 50, mysql);
    CalculatePoint2(70, 50, 120, 20, mysql);
    CalculatePoint2(120, 20, 70, 20, mysql);
    CalculatePoint2(70, 20, 70, 0, mysql);
}

/*获得主进程的进度条的计算进度*/
float New_GetMainPercentage()
{
    return((float)count1/CALCULATEAMOUNT1);
}

float New_GetChildPercentage()
{
    return ((float)count2/CALCULATEAMOUNT2);
}


/*
    子进程的函数体
*/
DWORD WINAPI subThread(LPVOID args)
{
    MYSQL mysql;

    mysql_init(&mysql);
    if(!mysql_real_connect(&mysql, "localhost", "root", "", "point", 0, NULL, 0))
    {
        fprintf(stderr, "%s2\n", mysql_error(&mysql));
        exit(-1);
    }
    fprintf(stdout, "%s\n", "数据库连接正确2\n");

    GetPoint2(mysql);
    mysql_close(&mysql);    /*关闭数据库*/

    return 0;
}

/*
    主进程的函数体
*/
void MainThread(void)
{
    MYSQL mysql;//用于连接数据库
    HANDLE ChildThread;

    mysql_init(&mysql);
    if(!mysql_real_connect(&mysql, "localhost", "root", "", "point", 0, NULL, 0))
    {
        fprintf(stderr, "数据库连接错误1\n", mysql_error(&mysql));
        exit(-1);
    }//end if

    fprintf(stdout, "%s\n", "数据库连接正确1");
    
    ChildThread = CreateThread(NULL, 0, subThread, NULL, 0, NULL);
    if(ChildThread == NULL)
    {
        fprintf(stderr, "%s\n", "子进程创建错误");
        exit(-1);
    }//end if
    else
    {
        fprintf(stdout, "%s\n", "子进程创建成功");
    }
    GetPoint1(mysql);
    mysql_close(&mysql);    /*关闭数据库*/

    WaitForSingleObject(ChildThread, INFINITE); /*等待子进程结束*/
}