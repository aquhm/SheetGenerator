# SheetGenerator

SheetGenerator는 구글 시트나 Excel 데이터를 바이너리 또는 JSON 형식으로 변환하고, 해당 데이터를 효율적으로 관리할 수 있는 C# 코드를 자동으로 생성하는 도구입니다.

## 주요 기능

- 구글 시트에서 데이터 가져오기
- 바이너리/JSON 형식으로 데이터 내보내기  
- 데이터 액세스를 위한 C# 코드 자동 생성
- MessagePack을 이용한 효율적인 바이너리 직렬화/역직렬화
- Index와 Key 기반 빠른 데이터 검색

## 의존성 패키지

```xml
<PackageReference Include="Google.Apis.Sheets.v4" Version="1.68.0.3624"/>
<PackageReference Include="MessagePack" Version="3.1.1"/>
<PackageReference Include="Microsoft.Extensions.Configuration" Version="9.0.0"/>
<PackageReference Include="Microsoft.Extensions.Configuration.Binder" Version="9.0.0"/>
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="9.0.0"/>
<PackageReference Include="Serilog" Version="4.2.1-dev-02337"/>
<PackageReference Include="Serilog.Sinks.Console" Version="6.0.0"/>
<PackageReference Include="Serilog.Sinks.File" Version="6.0.0"/>
<PackageReference Include="System.CommandLine" Version="2.0.0-beta4.22272.1"/>
```

## 사용법

1. Google Cloud Platform에서 서비스 계정 생성
2. 구글 시트 API 활성화
3. 구성 파일 설정 (settings.json)

```json
{
  "imports": [
    {
      "type": "GoogleSheet",
      "spreadsheetId": "YOUR_SPREADSHEET_ID",
      "auth": {
        "authType": "ServiceAccount",
        "serviceAccountPath": "credentials/service_account.json"
      }
    }
  ],
  "exports": [
    {
      "type": "Binary",
      "outputPath": "Generated/Data",
      "extension": "table"
    }
  ],
  "codeGens": [
    {
      "type": "CSharp",
      "fileLoadType": "binary",
      "namespace": "YourNamespace.Tables",
      "outputPath": "Generated/Script/CSharp",
      "templates": {
        "recordPath": "ScriptTemplate/CSharp/RecordTemplate.txt",  
        "tablePath": "ScriptTemplate/CSharp/TableTemplate.txt",
        "systemPath": "ScriptTemplate/CSharp/SystemTemplate.txt"
      }
    }
  ]
}
```

4. 구글 시트 포맷 설정

테이블은 다음과 같은 형식으로 구성됩니다:

![Google Sheet Example](https://example.com/path/to/image)

- 첫 줄: `@table:[테이블명]` 형식으로 테이블 시작을 표시
- 두 번째 줄: 테이블 설명
- 세 번째 줄: 컬럼명
- 네 번째 줄: 데이터 타입 (int, string, float, bool, double 등)
- 다섯 번째 줄: 컬럼 설명
- 이후: 실제 데이터

예시 스프레드시트의 구조:
```
@table:ClientTest
테스트 테이블입니다 (개발용)
Index    Key      Description   Column1    Column2    Column3    Column4
int      string   float        int        string     bool       double
인덱스(필수)  스트링 키  설명        설명1      설명2      설명3      설명4
1        key_1    1           1          a          true       123
2        key_2    2           2          b          false      123
...
```

5. 실행
```bash
SheetGenerator --config path/to/settings.json
```

6. 초기화
```bash
SheetGenerator --init
```

## 생성된 코드 사용 예시

```csharp
// 테이블 시스템 초기화
await TableSystem.Instance.InitializeAsync("path/to/data");

// Index로 데이터 접근
var record = TableSystem.Instance.YourTable.GetByIndex(1);

// Key로 데이터 접근 
var record = TableSystem.Instance.YourTable.GetByKey("unique_key");

// 모든 레코드 순회
foreach (var record in TableSystem.Instance.YourTable.Records)
{
    // 레코드 처리
}
```

## 주요 기능 설명

### 데이터 임포트
- 구글 시트나 Excel에서 데이터를 가져옵니다
- '@table:' 마커를 사용하여 테이블 구분
- 테이블별 컬럼 타입과 설명을 정의

### 데이터 익스포트
- Binary: MessagePack을 사용한 효율적인 바이너리 직렬화
- JSON: 가독성이 좋은 JSON 포맷으로 출력

### 코드 생성
- Record 클래스: 각 테이블 row를 표현
- Table 클래스: 테이블 전체 데이터와 조회 기능 제공
- TableSystem: 전체 테이블 관리 및 초기화 담당

## 주의사항

- 구글 시트에서는 '@table:' 형식으로 테이블 정의 필요
- Index와 Key 컬럼은 필수값
- 데이터 타입은 int, float, double, bool, string 지원
- 서비스 계정 인증 정보는 반드시 안전하게 관리

## 옵션

```bash
Options:
  --config <path>     설정 파일 경로
  --loglevel <level>  로그 레벨 (Verbose, Debug, Information, Warning, Error)
  --init             초기화 모드 실행
  --force            기존 파일 덮어쓰기
  --development      개발 모드로 실행
```

## 폴더 구조

```
SheetGenerator/
  ├── credentials/              # 인증 정보
  │   └── service_account.json
  ├── Generated/               # 생성된 파일들
  │   ├── Data/               # 익스포트된 데이터
  │   └── Script/             # 생성된 코드
  ├── ScriptTemplate/         # 코드 생성 템플릿
  │   └── CSharp/
  ├── settings.json           # 설정 파일
  └── SheetGenerator.exe      # 실행 파일
```

## License

MIT License