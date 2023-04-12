# Drone PID Controller
- Unity 엔진을 사용하여 PID Controller를 적용한 물리 기반 드론 시뮬레이션
- TwenyOz 기업에서 인턴 활동으로 제작한 프로젝트

## 개요
Unity엔진을 사용하여 만든 물리기반 드론 시뮬레이션입니다. 

실제 드론과 같이 4개의 모터가 가지는 힘에 따라 각각 모터에 개별적으로 힘을 가하여 주었고 
시계방향의 모터와 반시계 방향의 모터의 회전값에 따른 토크(Torque)값을 계산하여 드론 몸체에 주었습니다.

사용자 컨트롤러의 값에 따라 각각 모터의 힘을 설정해주어 드론이 기울거나 회전 하여 방향전환과 이동을 할 수 있도록 구현하였으며 
사용자가 쉽게 조작할 수 있도록 PID Controller를 구현하였습니다.

## PID Controller
PID 알고리즘은 Proportional, Integral, Derivative의 세 가지 항으로 이루어진 제어 알고리즘이다. 
이 알고리즘은 현재 상태와 목표 상태 간의 오차를 계산하고 이 오차를 이용하여 시스템의 출력을 조정하여 목표 상태에 근접하게 유지한다. 
각각의 항은 오차의 크기, 변화율, 누적치를 고려하여 제어 작업에 참여한다.

<img src ="https://user-images.githubusercontent.com/87575546/231389603-c7b6ca6b-14d7-4a13-9ca9-7a951bdd1573.png">




## Result
- ### Vertical Move
<img src="https://user-images.githubusercontent.com/87575546/231082115-52352da7-aa36-4473-b8bd-cf07cbd13734.gif" width="500" height="300"><br><br>

- ### Yaw
<img src="https://user-images.githubusercontent.com/87575546/231076952-2af77e77-f924-4e32-96a0-75f7a4511c15.gif" width="500" height="300"><br><br>

- ### Roll & Pitch
<img src="https://user-images.githubusercontent.com/87575546/231079434-afcaeb59-98b5-493d-b08f-673a6a887673.gif" width="500" height="300"><br><br>


- ### Roll & Pitch & Yaw
<img src="https://user-images.githubusercontent.com/87575546/231080035-6eecf87e-f055-4558-be6e-d471c1e09319.gif" width="500" height="300"><br><br>


- ### PID Control
<img src="https://user-images.githubusercontent.com/87575546/231077351-8529cf94-702e-40b0-a068-a29734999e14.gif" width="500" height="300">
<img src="https://user-images.githubusercontent.com/87575546/231077385-97b7b65b-acbd-4f4b-b6bf-610cd53ffa3e.gif" width="500" height="300">
