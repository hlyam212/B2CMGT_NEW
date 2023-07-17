using OracleHelper.TransactSql;
using EVABMS.AP.Parameter.Domain.Entities;
using EVABMS.AP.Parameter.Infrastructure;
using EVABMS.AP.Survey.Domain.Entities;
using OracleHelper.TransactSql;
using System.Transactions;
using System.Collections;
using System.Web;

namespace EVABMS.AP.Survey.Infrastructure
{
    public class SurveyRepository
    {
        public List<Form> Query()
        {
            OracleService ora = new OracleService();
            List<Form> result = ora.Select<Form>();
            return result;
        }

        public SurveyModel QuerySurveyModel (long id, string lang)
        {
            
            Form form = QueryForm(id,lang);
            List<Section> sections = QuerySection(id, lang);
            List<Question> questions = QueryQuestion(id, lang);
            List<Option> options = QueryOption(id, lang);

            questions = questions.Select(x => x.SetOPTION(options.Where(y => y.fk_form_mfoq_id == x.id).ToList())).ToList();

            sections = sections.Select(x => x.SetQUESTION(questions.Where(y => y.fk_mfos_id == x.id).ToList())).ToList();

            SurveyModel surveyModel = SurveyModel.Create(form, sections);

            return surveyModel;
        }

        public SurveyModel QueryBasic(long id, string lang)
        {

            Form form = QueryForm(id, lang);
            List<Section> sections = QuerySection(id, lang).Where(x => x.id == 1).ToList();
            List<Question> questions = QueryQuestion(id, lang);
            List<Option> options = QueryOption(id, lang);

            questions = questions.Select(x => x.SetOPTION(options.Where(y => y.fk_form_mfoq_id == x.id).ToList())).ToList();

            sections = sections.Select(x => x.SetQUESTION(questions.Where(y => y.fk_mfos_id == x.id).ToList())).ToList();

            SurveyModel surveyModel = SurveyModel.Create(form, sections);

            return surveyModel;
        }

        public Form QueryForm(long id, string lang) 
        {
            OracleService ora = new OracleService();
            Form form = new Form();
            form.SetQuery(id);
            Form aform = ora.SelectByPrimaryKey<Form>(form).FirstOrDefault();
            if (aform == null) 
            {
                return null;
            }

            ora = new OracleService();
            LangForm langform = new LangForm();
            langform.SetMFOFID(id, lang);
            List<LangForm> langforms = ora.SelectByIndex<LangForm>(langform, 1).ToList();
            if(langforms == null)
            {
                Form.Create(aform.id, aform.default_lang, aform.effective_start, aform.effective_end, aform.lastupdatedtimestamp, aform.lastupdateduserid, aform.env, null, null);
            }

            Form.Create(aform.id, aform.default_lang, aform.effective_start, aform.effective_end, aform.lastupdatedtimestamp, aform.lastupdateduserid, aform.env, langforms.Where(y => y.fk_mfof_id == aform.id).Select(y => y.key).FirstOrDefault(), langforms.Where(y => y.fk_mfof_id == aform.id).Select(y => y.value).FirstOrDefault());
            return aform;
        }

        public List<Section> QuerySection(long id, string lang)
        {
            OracleService ora = new OracleService();
            Section section = new Section();
            section.SetMFOFID(id);
            List<Section> sections = ora.SelectByIndex<Section>(section, 1).OrderBy(x => x.id).ToList();

            #region QueryLangSection
            ora = new OracleService();
            string sql = @" SELECT 
                                    L_SECTION.* 
                            FROM 
                                    MGT_LANG_FORM_SECTION L_SECTION
                            JOIN    MGT_FORM_SECTION SECTION   ON SECTION.ID=L_SECTION.FK_MFOS_ID
                            WHERE   SECTION.FK_MFOF_ID=:FK_MFOF_ID
                            AND     L_SECTION.LANG = :LANG";
            ora.SetOraParameters("FK_MFOF_ID", id.ToString(), OraDataType.Int64);
            ora.SetOraParameters("LANG", lang, OraDataType.Varchar2);
            List<LangSection> langsections = ora.Select<LangSection>(sql);
            #endregion

            sections = sections.Select(x => x.SetTITLE(langsections.Where(y => y.fk_mfos_id == x.id).Select(y => y.value).FirstOrDefault())).ToList();

            return sections;
        }

        public List<Question> QueryQuestion(long id, string lang)
        {
            OracleService ora = new OracleService();

            #region QueryQuestion
            string Q_sql = @"SELECT 
                                    QUESTION.* 
                           FROM 
                                    MGT_FORM_QUESTION QUESTION
                           JOIN MGT_FORM_SECTION SECTION ON SECTION.ID = QUESTION.FK_MFOS_ID
                           WHERE SECTION.FK_MFOF_ID=:FK_MFOF_ID
                           ORDER BY QUESTION.ID";
            ora.SetOraParameters("FK_MFOF_ID", id.ToString(), OraDataType.Int64);
            List<Question> questions = ora.Select<Question>(Q_sql);
            #endregion

            #region QueryLangQuestion
            ora = new OracleService();
            string LQ_sql = @" SELECT 
                                      L_QUESTION.*
                            FROM 
                                      MGT_LANG_FORM_QUESTION L_QUESTION
                            JOIN MGT_FORM_QUESTION QUESTION ON QUESTION.ID = L_QUESTION.FK_MFOQ_ID
                            JOIN MGT_FORM_SECTION SECTION ON SECTION.ID = QUESTION.FK_MFOS_ID
                            WHERE SECTION.FK_MFOF_ID=:FK_MFOF_ID
                            AND   L_QUESTION.LANG = :LANG
                            ORDER BY QUESTION.ID";
            ora.SetOraParameters("FK_MFOF_ID", id.ToString(), OraDataType.Int64);
            ora.SetOraParameters("LANG", lang, OraDataType.Varchar2);
            List<LangQuestion> langquestions = ora.Select<LangQuestion>(LQ_sql);
            #endregion

            questions = questions.Select(x => x.SetQUESTION(langquestions.Where(y => y.fk_mfoq_id == x.id).Select(y => y.value).FirstOrDefault())).ToList();

            return questions;
        }

        public List<Option> QueryOption(long id, string lang)
        {
            OracleService ora = new OracleService();

            #region QueryOption
            string O_sql = @" SELECT 
                                     OOPTION.*
                           FROM MGT_FORM_OPTION OOPTION 
                           JOIN MGT_FORM_QUESTION QUESTION ON QUESTION.ID = OOPTION.FK_FROM_MFOQ_ID
                           JOIN MGT_FORM_SECTION SECTION ON SECTION.ID = QUESTION.FK_MFOS_ID
                           WHERE SECTION.FK_MFOF_ID=:FK_MFOF_ID
                           ORDER BY OOPTION.ID";
            ora.SetOraParameters("FK_MFOF_ID", id.ToString(), OraDataType.Int64);
            List<Option> options = ora.Select<Option>(O_sql);
            #endregion

            #region QueryLangOption
            ora = new OracleService();
            string LO_sql = @"SELECT 
                                     L_OPTION.*
                            FROM MGT_LANG_FORM_OPTION L_OPTION
                            JOIN MGT_FORM_OPTION OOPTION ON OOPTION.ID = L_OPTION.FK_MFOO_ID
                            JOIN MGT_FORM_QUESTION QUESTION ON QUESTION.ID = OOPTION.FK_FROM_MFOQ_ID
                            JOIN MGT_FORM_SECTION SECTION ON SECTION.ID = QUESTION.FK_MFOS_ID
                            WHERE SECTION.FK_MFOF_ID=:FK_MFOF_ID
                            AND   L_OPTION.LANG = :LANG
                            ORDER BY L_OPTION.FK_MFOO_ID";
            ora.SetOraParameters("FK_MFOF_ID", id.ToString(), OraDataType.Int64);
            ora.SetOraParameters("LANG", lang, OraDataType.Varchar2);
            List<LangOption> langoptions = ora.Select<LangOption>(LO_sql);
            #endregion

            options = options.Select(x => x.SetOPTION(langoptions.Where(y => y.fk_mfoo_id == x.id).Select(y => y.value).FirstOrDefault())).ToList();

            return options;
        }

        public bool InsertAns(SurveyModel answer)
        {
            bool result;

            using (TransactionScope ts = new TransactionScope())
            {

                #region 新增表單資料
                OracleKeyService orakey = new OracleKeyService();

                long? mfaf_seq = orakey.GenerateKeyWithDual("MFAF_SEQ");

                AnsForm ansform = AnsForm.Create(mfaf_seq.Value, answer.form.default_lang, DateTime.Now, answer.form.id);

                OracleService oraService = new OracleService();
                result = oraService.Insert(ansform);
                if (result == false)
                {
                    return result;
                }
                #endregion

                #region 新增區塊資料
                orakey = new OracleKeyService();

                List<AnsSection> anssections = (from x in answer.section
                                                let _sectionID = orakey.GenerateKeyWithDual("MFAS_SEQ").Value
                                                select AnsSection.Create(_sectionID, ansform.id, x.id, DateTime.Now)).ToList();

                foreach (AnsSection x in anssections)
                {
                    oraService = new OracleService();
                    result = oraService.Insert(x);
                    if (result == false)
                    {
                        break;
                    }
                }
                if (result == false)
                {
                    return result;
                }

                #endregion

                #region 新增問題題目資料
                List<Question> questions = (from x in answer.section.Select(y => y.question)
                                            from y in x
                                            select Question.Create(y.id, y.must_id, y.question_type, y.lastupdatedtimestamp, y.lastupdateduserid, y.fk_mfos_id,y.fk_to_mfoq_id, y.seq, y.question, y.option)).ToList();


                orakey = new OracleKeyService();

                List<AnsQuestion> ansquestions = (from x in questions
                                                    let _questionID = orakey.GenerateKeyWithDual("MFAQ_SEQ").Value
                                                    select AnsQuestion.Create(_questionID, anssections.Where(y => y.fkmfosid == x.fk_mfos_id).Select(y => y.id).FirstOrDefault(), x.id, DateTime.Now)).ToList();


                foreach (AnsQuestion x in ansquestions)
                {
                    oraService = new OracleService();
                    result = oraService.Insert(x);
                    if (result == false)
                    {
                        break;
                    }
                }
                if (result == false)
                {
                    return result;
                }
                #endregion

                #region 新增回答選項資料
                List<Option> options = (from x in questions.Select(y => y.option)
                                        from z in x
                                        select Option.Create(z.id, z.lastupdatedtimestamp, z.lastupdateduserid, z.fk_form_mfoq_id, z.fk_to_mfoq_id,z.option_value, z.option, z.value)).ToList();

                orakey = new OracleKeyService();

                List<AnsOption> ansoptions = (from x in options
                                              where x.value != null
                                              let _optionID = orakey.GenerateKeyWithDual("MFAO_SEQ").Value
                                              select AnsOption.Create(_optionID, ansquestions.Where(y => y.fkmfoqid == x.fk_form_mfoq_id).Select(y => y.id).FirstOrDefault(), x.id, x.value, DateTime.Now)).ToList();

                foreach (AnsOption x in ansoptions)
                {
                    oraService = new OracleService();
                    result = oraService.Insert(x);
                    if (result == false)
                    {
                        break;
                    }
                }
                if (result == false)
                {
                    return result;
                }
                #endregion

                ts.Complete();
                return result;
            }
       
        }

        public double ComputingDays(string Now, string Dep)
        {
            return (DateTime.ParseExact(Now, "yyyyMMddHHmmss", null) - DateTime.ParseExact(Dep, "yyyyMMdd", null)).TotalDays;
        }

        public long QueryDrawCondition(string firstdate, string lastdate)
        {
            ParameterSetting quitirua = new ParameterSetting().SetQuery("EVABMS", "SURVEY-Q-FORMCS", "FLTDate");
            string fltdate = new ParameterRepository().Query(quitirua).FirstOrDefault().value.ToSafeString();
            quitirua = new ParameterSetting().SetQuery("EVABMS", "SURVEY-Q-FORMCS", "Class");
            string level = new ParameterRepository().Query(quitirua).FirstOrDefault().value.ToSafeString();
            OracleService ora = new OracleService();
            string sql = @"SELECT 
                                  A_QUESTION.FK_MFOQ_ID MFOQ_ID
                                  ,A_OPTION.value ANS
                                  ,A_QUESTION.ID MFAQ_ID
                                  ,A_OPTION.ID MFAO_ID
                           FROM MGT_FORM_ANS_OPTION A_OPTION
                           JOIN MGT_FORM_ANS_QUESTION  A_QUESTION ON A_OPTION.FK_MFAQ_SEQ  = A_QUESTION.ID
                           JOIN MGT_FORM_QUESTION L_QUESTION ON A_QUESTION.FK_MFOQ_ID = L_QUESTION.ID
                           WHERE A_QUESTION.FK_MFOQ_ID IN (:FK_MFOQ_ID)";
            //ora.SetOraParameters("FK_MFOQ_ID", fltdate + "," + level, OraDataType.Varchar2);

            List<string> data = ora.Select<string>(sql);
            #region QueryDrawCondition

            #endregion
            return 1;
        }

    }
}